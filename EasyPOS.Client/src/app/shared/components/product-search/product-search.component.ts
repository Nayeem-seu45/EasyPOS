import { Component, EventEmitter, Input, Output, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALIDATORS, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-product-search',
  templateUrl: './product-search.component.html',
  styleUrls: ['./product-search.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ProductSearchComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => ProductSearchComponent),
      multi: true
    }
  ],
})
export class ProductSearchComponent implements ControlValueAccessor {
  // General Properties
  @Input() label: string = '';
  @Input() placeholder: string = 'Search...';
  @Input() disabled: boolean = false; // Default: false
  @Input() readonly: boolean = false; // Default: false
  @Input() required: boolean = false; // Default: false
  @Input() autofocus: boolean = false; // Default: false
  @Input() autocomplete: string = 'off'; // Default: 'off'
  @Input() inputId: string = null; // Identifier for input focus
  @Input() name: string = null; // Name attribute of the input field

  // Data Handling Properties
  @Input() suggestions: any[] = []; // Array of suggestions to display
  @Input() field: string = 'label'; // Field of a suggested object
  @Input() optionLabel: string ; // Property name or getter function for label
  @Input() optionValue: string  = null; // Property name or getter function for value
  @Input() optionDisabled: string = null; // Disabled flag for options
  @Input() unique: boolean = true; // Ensures unique selection (Default: true)
  @Input() multiple: boolean = false; // Whether multiple values can be selected
  @Input() group: boolean = false; // Whether options are grouped (Default: false)
  @Input() dataKey: string = null; // Unique identifier for options

  // Search Behavior
  @Input() minLength: number = 1; // Minimum characters to trigger search
  @Input() delay: number = 300; // Delay between keystrokes
  @Input() forceSelection: boolean = false; // Enforce suggestions selection
  @Input() autoHighlight: boolean = false; // Auto-highlight first suggestion
  @Input() completeOnFocus: boolean = false; // Trigger query on focus
  @Input() searchLocale: boolean = false; // Use locale in searching
  @Input() focusOnHover: boolean = false; // Focus hovered option
  @Input() autoOptionFocus: boolean = false; // Focus first visible/selected element
  @Input() selectOnFocus: boolean = false; // Select focused option
  @Input() searchMessage: string = null; // Text when search is active
  @Input() emptySelectionMessage: string = null; // Text for no filtering results
  @Input() emptyAfterSelect: boolean = true; // Text for no filtering results

  // Appearance and Style
  @Input() style: {} = null; // Inline style for the component
  @Input() styleClass: string = null; // Style class for the component
  @Input() panelStyle: {} = null; // Style for the overlay panel
  @Input() panelStyleClass: string = null; // Style class for the overlay panel
  @Input() inputStyle: {} = null; // Style for the input field
  @Input() inputStyleClass: string = null; // Style class for the input field
  @Input() variant: 'outlined' | 'filled' = 'outlined'; // Input variant

  // Overlay and Panel Behavior
  @Input() scrollHeight: string = '200px'; // Max height of suggestions panel
  @Input() appendTo: any = null; // Attach overlay to a target element
  @Input() autoZIndex: boolean = true; // Manage layering automatically
  @Input() baseZIndex: number = 0; // Base zIndex value
  @Input() overlayOptions: {} = null; // Additional overlay configuration

  // Dropdown and Clear Button
  @Input() dropdown: boolean = false; // Show dropdown button
  @Input() dropdownIcon: string = 'pi pi-chevron-down'; // Dropdown icon
  @Input() dropdownMode: string = 'blank'; // Dropdown behavior
  @Input() showClear: boolean = false; // Show clear icon
  @Input() showEmptyMessage: boolean = true; // Show empty message

  // Virtual Scrolling
  @Input() virtualScroll: boolean = false; // Enable virtual scrolling
  @Input() virtualScrollItemSize: number = null; // Item size for virtual scrolling
  @Input() itemSize: number = null; // Element dimensions for options
  @Input() virtualScrollOptions: any = null; // Virtual scrolling configuration

  // Accessibility
  @Input() ariaLabel: string = null; // Accessibility label
  @Input() ariaLabelledBy: string = null; // IDs that label the input
  @Input() dropdownAriaLabel: string = null; // Accessibility label for dropdown

  // Transitions and Animations
  @Input() showTransitionOptions: string = '.12s cubic-bezier(0, 0, 0.2, 1)'; // Show animation
  @Input() hideTransitionOptions: string = '.1s linear'; // Hide animation

  // Miscellaneous
  @Input() maxlength: number = null; // Maximum characters in input
  @Input() tabindex: number = null; // Tab order
  @Input() size: number = null; // Input field size
  @Input() lazy: boolean = false; // Load data lazily

  // Internationalization Messages
  @Input() emptyMessage: string = null; // Default empty message
  @Input() selectionMessage: string = null; // Hidden message for selected options

  @Output() onComplete: EventEmitter<any> = new EventEmitter();
  @Output() onSelect: EventEmitter<any> = new EventEmitter();
  @Output() onUnselect: EventEmitter<any> = new EventEmitter();
  @Output() onDropdownClick: EventEmitter<any> = new EventEmitter();
  @Output() onClear: EventEmitter<any> = new EventEmitter();
  @Output() onKeyUp: EventEmitter<any> = new EventEmitter();

  value: any;
  onTouched: any = () => {};
  onChangeFn: any = (_: any) => {};

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

  handleComplete(event: any): void {
    this.onComplete.emit(event);
  }

  handleSelect(event: any): void {
    this.value = event.value;
    this.onChangeFn(this.value);
    this.onSelect.emit(event);
    if(this.emptyAfterSelect){
      setTimeout(() => {
        this.handleClear();
      }, 50);
    }
  }

  handleUnselect(event: any): void {
    this.onChangeFn(null);
    this.onUnselect.emit(event);
  }

  handleDropdownClick(event: any): void {
    this.onDropdownClick.emit(event);
  }

  handleClear(): void {
    this.value = null;
    this.onChangeFn(this.value);
    this.onClear.emit();
  }

  handleOnKeyUp(event){
    this.onKeyUp.emit(event);
  }
}
