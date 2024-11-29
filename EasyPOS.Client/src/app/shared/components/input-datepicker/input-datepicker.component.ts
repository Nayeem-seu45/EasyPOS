import { Component, EventEmitter, Input, Output, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, NG_VALIDATORS, ControlValueAccessor, Validator, AbstractControl, ValidationErrors } from '@angular/forms';

@Component({
  selector: 'app-input-datepicker',
  templateUrl: './input-datepicker.component.html',
  styleUrls: ['./input-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputDatepickerComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => InputDatepickerComponent),
      multi: true
    }
  ]
})
export class InputDatepickerComponent implements ControlValueAccessor, Validator {
  // General Properties
  @Input() label: string = '';
  @Input() placeholder: string = 'dd/mm/yyyy';
  @Input() required: boolean = false;
  @Input() hidden: boolean = false;
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;
  @Input() autofocus: boolean = false;
  @Input() name: string = null;
  @Input() inputId: string = null;
  @Input() showOnFocus: boolean = true;

  // Style & Accessibility
  @Input() inputStyle: { [key: string]: string } = {};
  @Input() inputStyleClass: string = null;
  @Input() panelStyle: { [key: string]: string } = null;
  @Input() panelStyleClass: string = null;
  @Input() style: {} = null;
  @Input() styleClass: string = null;
  @Input() ariaLabel: string = null;
  @Input() ariaLabelledBy: string = null;
  @Input() iconAriaLabel: string = null;

  // Date Formatting & Selection
  @Input() dateFormat: string = 'dd/mm/yy';
  @Input() selectionMode: 'single' | 'multiple' | 'range' = 'single';
  @Input() multipleSeparator: string = ',';
  @Input() rangeSeparator: string = '-';
  @Input() dataType: string = 'date';
  @Input() minDate: Date = null;
  @Input() maxDate: Date = null;
  @Input() yearRange: string = null;
  @Input() defaultDate: Date = null;
  @Input() firstDayOfWeek: number = null;
  @Input() maxDateCount: number = null;
  @Input() disabledDates: Date[] = null;
  @Input() disabledDays: number[] = null;

  // UI Features
  @Input() showIcon: boolean = true;
  @Input() icon: string = null;
  @Input() iconDisplay: 'input' | 'button' = 'input';
  @Input() readonlyInput: boolean = false;
  @Input() showButtonBar: boolean = true;
  @Input() showClear: boolean = false;
  @Input() todayButtonStyleClass: string = 'p-button-text';
  @Input() clearButtonStyleClass: string = 'p-button-text';
  @Input() monthNavigator: boolean = false;
  @Input() yearNavigator: boolean = false;
  @Input() appendTo: any = 'body';
  @Input() variant: "outlined" | "filled" = 'outlined';


  // Date Options
  @Input() stepYearPicker: number = 10;

  // Time Options
  @Input() showTime: boolean = false;
  @Input() timeOnly: boolean = false;
  @Input() hourFormat: '12' | '24' = '24';
  @Input() showSeconds: boolean = false;
  @Input() timeSeparator: string = ':';
  @Input() stepHour: number = 1;
  @Input() stepMinute: number = 1;
  @Input() stepSecond: number = 1;

  // Advanced Options
  @Input() shortYearCutoff: string | number = '+10';
  @Input() autoZIndex: boolean = true;
  @Input() baseZIndex: number = 0;
  @Input() responsiveOptions: any[] = null;
  @Input() keepInvalid: boolean = false;
  @Input() hideOnDateTimeSelect: boolean = true;
  @Input() inline: boolean = false;
  @Input() showOtherMonths: boolean = true;
  @Input() selectOtherMonths: boolean = false;
  @Input() numberOfMonths: number = 1;
  @Input() locale: any = null;
  @Input() view: "date" | "month" | "year" = "date";
  @Input() touchUI: boolean = false;

  // Accessibility & Navigation
  @Input() tabindex: number = null;
  @Input() focusTrap: boolean = true;
  @Input() showWeek: boolean = false;
  @Input() startWeekFromFirstDayOfYear: boolean = false;
  @Input() showTransitionOptions: string = '.12s cubic-bezier(0, 0, 0.2, 1)';
  @Input() hideTransitionOptions: string = '.1s linear';

  // Output Event Emitters
  @Output() onFocus: EventEmitter<Event> = new EventEmitter<Event>();
  @Output() onBlur: EventEmitter<Event> = new EventEmitter<Event>();
  @Output() onClose: EventEmitter<AnimationEvent> = new EventEmitter<AnimationEvent>();
  @Output() onSelect: EventEmitter<Date> = new EventEmitter<Date>();
  @Output() onClear: EventEmitter<any> = new EventEmitter<any>();
  @Output() onInput: EventEmitter<any> = new EventEmitter<any>();
  @Output() onTodayClick: EventEmitter<Date> = new EventEmitter<Date>();
  @Output() onClearClick: EventEmitter<any> = new EventEmitter<any>();
  @Output() onMonthChange: EventEmitter<any> = new EventEmitter<any>();
  @Output() onYearChange: EventEmitter<any> = new EventEmitter<any>();
  @Output() onClickOutside: EventEmitter<any> = new EventEmitter<any>();
  @Output() onShow: EventEmitter<any> = new EventEmitter<any>()
  @Output() onChange: EventEmitter<any> = new EventEmitter<any>()

  // Callback for ControlValueAccessor
  value: any = null;
  onTouched: any = () => { };
  onChangeFn: any = (_: any) => { };

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  validate(control: AbstractControl): ValidationErrors | null {
    if (this.required && (control.value === null || control.value === '')) {
      return { required: true };
    }
    return null;
  }

  // Event Handlers
  onBlurEvent(event: Event): void {
    this.onBlur.emit(event);
    this.onTouched();
  }

  onFocusEvent(event: Event): void {
    this.onFocus.emit(event);
  }

  onCloseEvent(event: any): void {
    this.onClose.emit(event);
  }

  onSelectEvent(date: Date): void {
    this.onSelect.emit(date);
  }

  onClearEvent(value: any): void {
    this.value = null; // Reset the value
    this.onChangeFn(this.value); // Propagate the change to the parent form
    this.onClear.emit(value);
  }

  onInputEvent(value: any): void {
    this.onInput.emit(value);
  }

  onTodayClickEvent(date: Date): void {
    this.onTodayClick.emit(date);
  }

  onClearClickEvent(value: any): void {
    this.onClearClick.emit(value);
  }

  onMonthChangeEvent(event: any): void {
    this.onMonthChange.emit(event);
  }

  onYearChangeEvent(event: any): void {
    this.onYearChange.emit(event);
  }

  onClickOutsideEvent(value: any): void {
    this.onClickOutside.emit(value);
  }

  onShowEvent(value: any): void {
    this.onShow.emit(value);
  }

  onInputChange(event: any): void {
    this.value = event;
    this.onChangeFn(this.value);
    this.onChange.emit(this.value);
  }
}