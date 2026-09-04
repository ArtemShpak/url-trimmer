import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { UrlService } from '../../../core/services/url';
import { ShortUrl } from '../../../core/models/url';

@Component({
  selector: 'app-url-info',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './url-info.html',
  styleUrl: './url-info.scss',
})
export class UrlInfoComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private urlService = inject(UrlService);

  url = signal<ShortUrl | null>(null);
  errorMessage = signal<string>('');

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.urlService.getById(id).subscribe({
        next: (data) => this.url.set(data),
        error: () => this.errorMessage.set('Посилання не знайдено'),
      });
    }
  }
}
