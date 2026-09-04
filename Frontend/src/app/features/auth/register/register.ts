import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = { email: '', password: '' };
  errorMessage = '';

  onSubmit() {
    this.authService.register(this.credentials).subscribe({
      next: () => {
        alert('Реєстрація успішна! Тепер ви можете увійти.');
        this.errorMessage = '';
        this.router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage = 'Помилка реєстрації. Користувач вже існує або пароль заслабкий.';
      },
    });
  }
}
