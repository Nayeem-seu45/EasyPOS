import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ChartComponent } from './components/chart/chart.component';
import { DashCardComponent } from './components/dash-card/dash-card.component';

export const routes: Routes = [
  {path: '', component: DashboardComponent}
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRouting{

}

export const DashboardRoutingComponents = [
  DashboardComponent,
  ChartComponent,
  DashCardComponent
]