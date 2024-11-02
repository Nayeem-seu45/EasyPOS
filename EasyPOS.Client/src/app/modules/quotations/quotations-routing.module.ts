import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuotationListComponent } from './components/quotation-list/quotation-list.component';
import { QuotationDetailComponent } from './components/quotation-detail/quotation-detail.component';
import { UpdateQuotationDetailComponent } from './components/update-quotation-detail/update-quotation-detail.component';

const routes: Routes = [
  {path: 'create', component: QuotationDetailComponent},
  {path: 'list', component: QuotationListComponent},
  {path: 'edit/:id', component: QuotationDetailComponent},
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class QuotationsRoutingModule { }


export const quotationRoutingComponents = [
  QuotationListComponent,
  QuotationDetailComponent,
  UpdateQuotationDetailComponent
]
