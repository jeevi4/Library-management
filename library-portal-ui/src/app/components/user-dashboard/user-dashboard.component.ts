import { Component, OnInit } from '@angular/core';
import { BorrowService } from '../../services/borrow.service';
import { AuthService } from '../../services/auth.service';
import { BorrowRecord } from '../../models/borrow.model';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.css']
})
export class UserDashboardComponent implements OnInit {
  borrowHistory: BorrowRecord[] = [];
  message = '';
  isError = false;

  constructor(
    public authService: AuthService,
    private borrowService: BorrowService
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.borrowService.getMyHistory().subscribe(records => {
      this.borrowHistory = records;
    });
  }

  onReturn(borrowId: number): void {
    this.borrowService.returnBook(borrowId).subscribe({
      next: () => {
        this.message = 'Book returned successfully!';
        this.isError = false;
        this.loadHistory();
        setTimeout(() => this.message = '', 3000);
      },
      error: (err) => {
        this.message = err.error || 'Failed to return book.';
        this.isError = true;
      }
    });
  }
}