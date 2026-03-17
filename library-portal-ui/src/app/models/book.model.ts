export interface Book {
  bookId: number;
  title: string;
  author: string;
  category: string;
  publishedYear: number;
  availabilityStatus: string;
}

export interface BookRequest {
  title: string;
  author: string;
  category: string;
  publishedYear: number;
}