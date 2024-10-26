import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

type ChartType = "bar" | "line" | "scatter" | "bubble" | "pie" | "doughnut" | "polarArea" | "radar";

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrl: './chart.component.scss'
})
export class ChartComponent implements OnChanges {
  @Input() type: string = null;
  @Input() data: any = null;
  @Input() options: any = null;
  @Input() plugins: any[] = null;
  @Input() width: string = null;
  @Input() height: string = null;
  @Input() responsive: boolean = true;

  @Output() onDataSelect = new EventEmitter<any>();

  chartType: ChartType;

  ngOnChanges(changes: SimpleChanges): void {
    this.chartType = this.type as ChartType;
  }

  handleDataSelect(event: any){
    this.onDataSelect.emit(event);
  }

}
