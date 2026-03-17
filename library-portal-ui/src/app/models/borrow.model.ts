export interface BorrowRecord {
  borrowId: number;
  borrowDate: string;
  returnDate: string | null;
  status: string;
  book: {
    bookId: number;
    title: string;
    author: string;
    category: string;
  };
}

export interface DashboardStats {
  totalBooks: number;
  totalUsers: number;
  borrowedBooks: number;
  availableBooks: number;
  totalBorrows: number;
}