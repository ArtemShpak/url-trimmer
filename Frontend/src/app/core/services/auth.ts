import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, finalize } from 'rxjs';
import { switchMap, tap } from 'rxjs/operators';
import { UserProfile } from '../models/user-profile';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5068/api/auth';

  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isLoadingSubject = new BehaviorSubject<boolean>(true);
  public isLoading$ = this.isLoadingSubject.asObservable();

  constructor() {
    this.loadUserProfile();
  }

  get currentUser(): UserProfile | null {
    return this.currentUserSubject.value;
  }

  login(credentials: any): Observable<UserProfile> {
    return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
      switchMap(() => this.http.get<UserProfile>(`${this.apiUrl}/me`)),
      tap((user) => this.currentUserSubject.next(user)),
    );
  }

  register(credentials: any) {
    return this.http.post(`${this.apiUrl}/registration`, credentials);
  }

  logout() {
    return this.http
      .post(`${this.apiUrl}/logout`, {})
      .pipe(tap(() => this.currentUserSubject.next(null)));
  }

  loadUserProfile() {
    this.isLoadingSubject.next(true);
    this.http
      .get<UserProfile>(`${this.apiUrl}/me`)
      .pipe(finalize(() => this.isLoadingSubject.next(false)))
      .subscribe({
        next: (user) => this.currentUserSubject.next(user),
        error: () => this.currentUserSubject.next(null),
      });
  }
}
