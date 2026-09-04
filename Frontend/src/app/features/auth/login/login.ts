import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = { email: '', password: '' };
  errorMessage = '';

  onSubmit() {
    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.errorMessage = '';
        this.router.navigate(['/urls']);
      },
      error: () => {
        this.errorMessage = 'Неправильний email або пароль';
      },
    });
  }
}
