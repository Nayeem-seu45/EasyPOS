import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { ExpensesClient } from 'src/app/modules/generated-clients/api-service';

@Component({
  selector: 'app-expense-detail',
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: ExpensesClient}]
})
export class ExpenseDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: ExpensesClient){
    super(entityClient)
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      expenseDate: [null],
      title: [null],
      referenceNo: [null],
      warehouseId: [null],
      categoryId: [null],
      amount: [null],
      accountId: [null],
      description: [null],
      attachmentUrl: [null]

    });
  }

}
