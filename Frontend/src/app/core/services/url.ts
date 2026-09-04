import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ShortUrl } from '../models/url';

@Injectable({ providedIn: 'root' })
export class UrlService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5068/api/shorturls';

  getAll() {
    return this.http.get<ShortUrl[]>(this.apiUrl);
  }

  create(originalUrl: string) {
    return this.http.post<ShortUrl>(this.apiUrl, { originalUrl });
  }

  delete(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getById(id: number) {
    return this.http.get<ShortUrl>(`${this.apiUrl}/${id}`);
  }
}
