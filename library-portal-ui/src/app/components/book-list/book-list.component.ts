import { Component, OnInit } from '@angular/core';
import { BookService } from '../../services/book.service';
import { BorrowService } from '../../services/borrow.service';
import { AuthService } from '../../services/auth.service';
import { Book, BookRequest } from '../../models/book.model';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  searchQuery = '';
  message = '';
  isError = false;

  // Admin book form
  showForm = false;
  editingBook: Book | null = null;
  bookForm: BookRequest = { title: '', author: '', category: '', publishedYear: 0 };

  constructor(
    public bookService: BookService,
    public borrowService: BorrowService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.bookService.getAllBooks().subscribe(books => this.books = books);
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) {
      this.loadBooks();
      return;
    }
    this.bookService.searchBooks(this.searchQuery)
      .subscribe(books => this.books = books);
  }

  onBorrow(bookId: number): void {
    this.borrowService.borrowBook(bookId).subscribe({
      next: () => {
        this.showMessage('Book borrowed successfully!', false);
        this.loadBooks();
      },
      error: (err) => this.showMessage(err.error || 'Failed to borrow.', true)
    });
  }

  onDelete(bookId: number): void {
    if (!confirm('Delete this book?')) return;
    this.bookService.deleteBook(bookId).subscribe({
      next: () => {
        this.showMessage('Book deleted.', false);
        this.loadBooks();
      },
      error: (err) => this.showMessage(err.error || 'Failed to delete.', true)
    });
  }

  openAddForm(): void {
    this.editingBook = null;
    this.bookForm = { title: '', author: '', category: '', publishedYear: 0 };
    this.showForm = true;
  }

  openEditForm(book: Book): void {
    this.editingBook = book;
    this.bookForm = {
      title: book.title,
      author: book.author,
      category: book.category,
      publishedYear: book.publishedYear
    };
    this.showForm = true;
  }

  onSaveBook(): void {
    if (this.editingBook) {
      this.bookService.updateBook(this.editingBook.bookId, this.bookForm).subscribe({
        next: () => { this.showMessage('Book updated!', false); this.showForm = false; this.loadBooks(); },
        error: () => this.showMessage('Failed to update.', true)
      });
    } else {
      this.bookService.addBook(this.bookForm).subscribe({
        next: () => { this.showMessage('Book added!', false); this.showForm = false; this.loadBooks(); },
        error: () => this.showMessage('Failed to add.', true)
      });
    }
  }

  showMessage(msg: string, isError: boolean): void {
    this.message = msg;
    this.isError = isError;
    setTimeout(() => this.message = '', 3000);
  }
}