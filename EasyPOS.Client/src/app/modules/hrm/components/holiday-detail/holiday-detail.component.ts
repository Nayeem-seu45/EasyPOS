import { Component, Inject } from '@angular/core';
import { BaseDetailComponent } from 'src/app/shared/components/base-detail/base-detail.component';
import { ENTITY_CLIENT } from 'src/app/shared/injection-tokens/tokens';
import { HolidaysClient } from 'src/app/modules/generated-clients/api-service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-holiday-detail',
  templateUrl: './holiday-detail.component.html',
  styleUrl: './holiday-detail.component.scss',
  providers: [{provide: ENTITY_CLIENT, useClass: HolidaysClient}, DatePipe]
})
export class HolidayDetailComponent extends BaseDetailComponent {

  constructor(@Inject(ENTITY_CLIENT) entityClient: HolidaysClient,
  private datePipe: DatePipe){
    super(entityClient)
  }

  override beforeActionProcess(command: any): any {
    return {
      ...command,
      startDate: this.datePipe.transform(command.startDate, 'yyyy-MM-dd'),
      endDate: this.datePipe.transform(command.endDate, 'yyyy-MM-dd'),
    };
  }

  override initializeFormGroup() {
    this.form = this.fb.group({
      id: [null],
      title: [null],
      startDate: [null],
      endDate: [null],
      description: [null],
      isActive: [true]

    });
  }

}
