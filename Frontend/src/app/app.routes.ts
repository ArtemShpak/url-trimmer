import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { UrlListComponent } from './features/urls/url-list/url-list';
import { UrlInfoComponent } from './features/urls/url-info/url-info';
import { authGuard } from './core/guards/auth-guard'; // Додано

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'urls', component: UrlListComponent },
  { path: 'urls/:id', component: UrlInfoComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/urls', pathMatch: 'full' },
  { path: '**', redirectTo: '/urls' },
];
