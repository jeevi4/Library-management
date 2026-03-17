import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BorrowRecord, DashboardStats } from '../models/borrow.model';

@Injectable({
  providedIn: 'root'
})
export class BorrowService {
  private apiUrl = 'http://localhost:5113/api/Borrow';

  constructor(private http: HttpClient) {}

  borrowBook(bookId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${bookId}`, {});
  }

  returnBook(borrowId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/return/${borrowId}`, {});
  }

  getMyHistory(): Observable<BorrowRecord[]> {
    return this.http.get<BorrowRecord[]>(`${this.apiUrl}/my-history`);
  }

  getAllBorrows(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/all`);
  }

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard`);
  }
}