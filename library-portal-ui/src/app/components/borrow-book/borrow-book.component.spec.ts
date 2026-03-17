import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BorrowBookComponent } from './borrow-book.component';

describe('BorrowBookComponent', () => {
  let component: BorrowBookComponent;
  let fixture: ComponentFixture<BorrowBookComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BorrowBookComponent]
    });
    fixture = TestBed.createComponent(BorrowBookComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
