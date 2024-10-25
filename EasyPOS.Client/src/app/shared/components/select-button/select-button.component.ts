import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { NG_VALUE_ACCESSOR, NG_VALIDATORS, ControlValueAccessor, Validator, AbstractControl, ValidationErrors } from '@angular/forms';

@Component({
  selector: 'app-select-button',
  templateUrl: './select-button.component.html',
  styleUrl: './select-button.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectButtonComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => SelectButtonComponent),
      multi: true
    }
  ]
})
export class SelectButtonComponent implements ControlValueAccessor, Validator {
  @Input() label: string = '';
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;
  @Input() hidden: boolean = false;
  @Input() options: any[] = [];
  @Input() optionLabel: string = 'name';
  @Input() optionValue: string = 'id';
  @Input() optionDisabled: string = null;
  @Input() unselectable: boolean = false;
  @Input() tabindex: number = 0;
  @Input() multiple: boolean = false;
  @Input() allowEmpty: boolean = false;
  @Input() style: any = null;
  @Input() styleClass: string = null;
  @Input() dataKey: string = null;
  @Input() autofocus: boolean = false;
  @Input() showOnlyIcon: boolean = false;

  @Output() onOptionClick = new EventEmitter<any>();

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
    return this.required && (!this.value || this.value.length === 0) ? { required: true } : null;
  }

  onInputChange(event: any): void {
    this.value = event.value;
    this.onChangeFn(this.value);
  }

  handleOptionClick(event: any): void {
    this.onOptionClick.emit(event);
  }
  

  onBlurEvent(): void {
    this.onTouched();
  }
}
