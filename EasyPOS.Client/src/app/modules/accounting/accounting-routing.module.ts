import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountDetailComponent } from './components/account-detail/account-detail.component';
import { AccountListComponent } from './components/account-list/account-list.component';
import { MoneyTransferDetailComponent } from './components/money-transfer-detail/money-transfer-detail.component';
import { ExpenseListComponent } from './components/expense-list/expense-list.component';
import { ExpenseDetailComponent } from './components/expense-detail/expense-detail.component';
import { MoneyTransferListComponent } from './components/money-transfer-list/money-transfer-list.component';

const routes: Routes = [
  {path: 'accounts', component: AccountListComponent},
  {path: 'money-transfer', component: MoneyTransferListComponent},
  {path: 'expenses', component: ExpenseListComponent},
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountingRoutingModule { }

export const AccountingRoutingComponents = [
  AccountListComponent,
  AccountDetailComponent,
  MoneyTransferListComponent,
  MoneyTransferDetailComponent,
  ExpenseListComponent,
  ExpenseDetailComponent
]
