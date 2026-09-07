import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { distinctUntilChanged, switchMap } from 'rxjs/operators';
import { AuthService } from '../../../core/services/auth';
import { UrlService } from '../../../core/services/url';
import { ShortUrl } from '../../../core/models/url';

@Component({
  selector: 'app-url-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './url-list.html',
  styleUrl: './url-list.scss',
})
export class UrlListComponent implements OnInit {
  public authService = inject(AuthService);
  private urlService = inject(UrlService);
  private destroyRef = inject(DestroyRef);

  urls = signal<ShortUrl[]>([]);
  newOriginalUrl = '';
  errorMessage = '';
  isLoading = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  isDeleting = signal<number | null>(null);

  ngOnInit() {
    this.isLoading.set(true);

    this.authService.currentUser$
      .pipe(
        distinctUntilChanged((prev, curr) => prev?.id === curr?.id),
        switchMap(() => this.urlService.getAll()),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (data) => {
          this.urls.set(data);
          this.isLoading.set(false);
          this.errorMessage = '';
        },
        error: (err) => {
          console.error('Failed to load URLs', err);
          this.isLoading.set(false);
          this.errorMessage = 'Не вдалося завантажити посилання.';
        },
      });
  }

  addUrl() {
    const trimmedUrl = this.newOriginalUrl.trim();
    if (!trimmedUrl) return;

    this.errorMessage = '';
    this.isSubmitting.set(true);

    this.urlService.create(trimmedUrl).subscribe({
      next: (newUrl: ShortUrl) => {
        this.urls.update((current) => [newUrl, ...current]);
        this.newOriginalUrl = '';
        this.isSubmitting.set(false);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const errorCode = err.error?.code;

        if (errorCode === 'Url.AlreadyExists') {
          this.errorMessage = 'Такий URL вже існує в системі.';
        } else if (errorCode === 'Auth.Unauthorized') {
          this.errorMessage = 'Будь ласка, увійдіть у систему знову.';
        } else {
          this.errorMessage = err.error?.error || 'Сталася непередбачувана помилка.';
        }
      },
    });
  }

  canDelete(url: ShortUrl): boolean {
    const user = this.authService.currentUser;
    if (!user) return false;
    return user.role === 'Admin' || user.id === url.createdByUserId;
  }

  deleteUrl(id: number) {
    if (!confirm('Ви впевнені, що хочете видалити це посилання?')) {
      return;
    }

    this.isDeleting.set(id);

    this.urlService.delete(id).subscribe({
      next: () => {
        this.urls.update((current) => current.filter((u) => u.id !== id));
        this.isDeleting.set(null);
      },
      error: (err) => {
        this.isDeleting.set(null);

        if (err.status === 403) {
          this.errorMessage = 'У вас немає дозволу видаляти це посилання.';
        } else if (err.status === 401) {
          this.errorMessage = 'Сесія завершилася. Увійдіть знову.';
        } else {
          this.errorMessage = 'Не вдалося видалити посилання.';
        }
      },
    });
  }
}
