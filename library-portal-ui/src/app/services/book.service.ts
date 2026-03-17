import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book, BookRequest } from '../models/book.model';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'http://localhost:5113/api/Books';

  constructor(private http: HttpClient) {}

  getAllBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl);
  }

  searchBooks(query: string): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/search?query=${query}`);
  }

  getBook(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  addBook(book: BookRequest): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, book);
  }

  updateBook(id: number, book: BookRequest): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/${id}`, book);
  }

  deleteBook(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}