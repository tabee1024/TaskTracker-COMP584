// src/app/guards/auth.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  // Browser-only (SSR is disabled now, but this is still safe)
  if (typeof window === 'undefined') {
    return router.parseUrl('/login');
  }

  const token = localStorage.getItem('token');

  // If no token, redirect to login
  if (!token) {
    return router.parseUrl('/login');
  }

  return true;
};
