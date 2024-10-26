import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Subscription, debounceTime } from 'rxjs';
import { LayoutService } from 'src/app/layout/service/app.layout.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {

  items!: MenuItem[];

  chartData: any;

  chartOptions: any;

  subscription!: Subscription;

  customCard: any;
  revenueCard: any;
  customerCard: any;
  commentsCard: any;

  cardList: any[] = [];

  constructor(public layoutService: LayoutService) {
    this.subscription = this.layoutService.configUpdate$
      .pipe(debounceTime(25))
      .subscribe((config) => {
        this.initChart();
      });
  }

  ngOnInit() {
    this.initChart();

    this.items = [
      { label: 'Add New', icon: 'pi pi-fw pi-plus' },
      { label: 'Remove', icon: 'pi pi-fw pi-minus' }
    ];

    this.customCard = {
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

    this.revenueCard = {
      data: {
        title: 'Revenue',
        content: '$2,100',
        footerLeft: '%52+',
        footerRight: 'since last week',
      },
      options: {
        titleClass: 'text-500 font-medium mb-3',
        contentClass: 'text-900 font-medium text-xl',
        icon: 'pi pi-map-marker text-orange-500 text-xl',
        iconBgClass: 'bg-orange-100 border-round',
        footerLeftClass: 'text-green-500 font-medium',
        footerRightClass: 'text-500'
      }
    };

    this.customerCard = {
      data: {
        title: 'Customers',
        content: '28,441',
        footerLeft: '520',
        footerRight: 'newly registered',
      },
      options: {
        titleClass: 'text-500 font-medium mb-3',
        contentClass: 'text-900 font-medium text-xl',
        icon: 'pi pi-inbox text-cyan-500 text-xl',
        iconBgClass: 'bg-cyan-100 border-round',
        footerLeftClass: 'text-green-500 font-medium',
        footerRightClass: 'text-500'
      }
    };

    this.commentsCard = {
      data: {
        title: 'Comments',
        content: '152 Unread',
        footerLeft: '85',
        footerRight: 'responded',
      },
      options: {
        titleClass: 'text-500 font-medium mb-3',
        contentClass: 'text-900 font-medium text-xl',
        icon: 'pi pi-comment text-purple-500 text-xl',
        iconBgClass: 'bg-purple-100 border-round',
        footerLeftClass: 'text-green-500 font-medium',
        footerRightClass: 'text-500'
      }
    };

    this.cardList = [this.customCard, this.revenueCard, this.customerCard, this.commentsCard];

  }

  // Drag & Drop
draggingCardIndex: number | null = null;

onDragStart(index: number, event: MouseEvent) {
    this.draggingCardIndex = index;
}

onDragEnd() {
  this.draggingCardIndex = null; // Reset the dragging index
}

onCardDrop(targetIndex: number) {
  if (this.draggingCardIndex !== null && this.draggingCardIndex !== targetIndex) {
    const cardToMove = this.cardList[this.draggingCardIndex];

    // Remove card from original position
    this.cardList.splice(this.draggingCardIndex, 1);

    // Insert card at target position
    this.cardList.splice(targetIndex, 0, cardToMove);

    // Reset the index
    this.draggingCardIndex = null;
  }
}

  initChart() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    this.chartData = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label: 'First Dataset',
          data: [65, 59, 80, 81, 56, 55, 40],
          fill: false,
          backgroundColor: documentStyle.getPropertyValue('--bluegray-700'),
          borderColor: documentStyle.getPropertyValue('--bluegray-700'),
          tension: .4
        },
        {
          label: 'Second Dataset',
          data: [28, 48, 40, 19, 86, 27, 90],
          fill: false,
          backgroundColor: documentStyle.getPropertyValue('--green-600'),
          borderColor: documentStyle.getPropertyValue('--green-600'),
          tension: .4
        }
      ]
    };

    this.chartOptions = {
      plugins: {
        legend: {
          labels: {
            color: textColor
          }
        }
      },
      scales: {
        x: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        },
        y: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        }
      }
    };
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
