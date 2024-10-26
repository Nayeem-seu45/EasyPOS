import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-dash-card',
  templateUrl: './dash-card.component.html',
  styleUrl: './dash-card.component.scss'
})
export class DashCardComponent {
  @Input() data: any;
  @Input() options: any;

  // Sample for demo
  customCardOptions = {
    data: {
      title: 'Custom Card',
      content: '250.21',
      footerLeft: '21.05% increased',
      footerRight: 'last week',
    },
    options: {
      titleClass: 'text-500 font-medium mb-3',
      contentClass: 'text-blue-100 text-xl font-medium',
      icon: 'pi pi-shopping-cart text-blue-500 text-xl',
      iconBgClass: ' bg-blue-100 border-round',
      footerLeftClass: 'text-green-500 font-medium',
      footerRightClass: 'text-500'
    }
  }

}
