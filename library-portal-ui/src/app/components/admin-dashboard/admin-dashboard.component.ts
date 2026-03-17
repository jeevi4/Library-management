import { Component, OnInit } from '@angular/core';
import { BorrowService } from '../../services/borrow.service';
import { DashboardStats } from '../../models/borrow.model';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  allBorrows: any[] = [];

  constructor(private borrowService: BorrowService) {}

  ngOnInit(): void {
    this.borrowService.getDashboardStats().subscribe(s => this.stats = s);
    this.borrowService.getAllBorrows().subscribe(b => this.allBorrows = b);
  }
}