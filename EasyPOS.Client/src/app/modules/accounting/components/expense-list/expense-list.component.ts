import { Component, inject } from '@angular/core';
import { ExpenseDetailComponent } from '../expense-detail/expense-detail.component';
import { ExpensesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-expense-list',
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.scss',
  providers: [ExpensesClient]
})
export class ExpenseListComponent {
  detailComponent = ExpenseDetailComponent;
  pageId = 'fb452c4b-f366-4179-b3bf-08dcf782b6da'

  entityClient: ExpensesClient = inject(ExpensesClient);
}
