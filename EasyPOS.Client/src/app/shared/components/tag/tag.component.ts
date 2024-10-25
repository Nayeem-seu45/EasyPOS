import { Component, Input, OnChanges  } from '@angular/core';

type SeverityType = 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast';

@Component({
  selector: 'app-tag',
  templateUrl: './tag.component.html',
  styleUrl: './tag.component.scss'
})
export class TagComponent implements OnChanges  {
  @Input() style: {} = null;
  @Input() styleClass: string = null;
  @Input() severity: string = null;
  @Input() value: string = null;
  @Input() icon: string = null;
  @Input() rounded: boolean = false;

  severityType: SeverityType = 'info';

  ngOnChanges() {
    this.severityType = this.severity as SeverityType;
    console.log(this.severityType);
  } 
}
