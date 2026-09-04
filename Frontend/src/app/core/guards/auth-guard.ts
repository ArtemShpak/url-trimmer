import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { filter, map, switchMap, take } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoading$.pipe(
    filter((isLoading) => !isLoading),
    take(1),
    switchMap(() => authService.currentUser$),
    take(1),
    map((user) => {
      if (user) {
        return true;
      }
      return router.createUrlTree(['/login']);
    }),
  );
};
