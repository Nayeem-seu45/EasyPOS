import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { ToastComponent } from './components/toast/toast.component';
import { ToastModule } from 'primeng/toast';
import { DialogService } from 'primeng/dynamicdialog';
import { CustomDialogService } from './services/custom-dialog.service';
import { DataGridComponent } from './components/data-grid/data-grid.component';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { FileUploadModule } from 'primeng/fileupload';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { InputTextComponent } from './components/input-text/input-text.component';
import { ValidatorMsgComponent } from './components/validator-msg/validator-msg.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberComponent } from './components/input-number/input-number.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputCurrencyComponent } from './components/input-currency/input-currency.component';
import { InputDecimalComponent } from './components/input-decimal/input-decimal.component';
import { CalendarModule } from 'primeng/calendar';
import { InputDateComponent } from './components/input-date/input-date.component';
import { InputTimeComponent } from './components/input-time/input-time.component';
import { InputYearComponent } from './components/input-year/input-year.component';
import { InputTextAreaComponent } from './components/input-text-area/input-textarea.component';
import { InputSwitchComponent } from './components/input-switch/input-switch.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CheckboxModule } from 'primeng/checkbox';
import { InputCheckboxComponent } from './components/input-checkbox/input-checkbox.component';
import { InputSelectComponent } from './components/input-select/input-select.component';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputRadioComponent } from './components/input-radio/input-radio.component';
import { InputMultiselectComponent } from './components/input-multiselect/input-multiselect.component';
import { InputColorComponent } from './components/input-color/input-color.component';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputFileComponent } from './components/input-file/input-file.component';
import { InputMaskModule } from 'primeng/inputmask';
import { InputMaskComponent } from './components/input-mask/input-mask.component';
import { PasswordModule } from 'primeng/password';
import { InputPasswordComponent } from './components/input-password/input-password.component';
import { InputEditorComponent } from './components/input-editor/input-editor.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ButtonComponent } from './components/button/button.component';
import { TreeModule } from 'primeng/tree';
import { TreeComponent } from './components/tree/tree.component';
import { TreeSelectModule } from 'primeng/treeselect';
import { TreeSelectComponent } from './components/tree-select/tree-select.component';
import { ToastService } from './services/toast.service';
import { InputFileAdvComponent } from './components/input-file-advanced/input-file-adv.component';
import { SelectButtonModule } from 'primeng/selectbutton';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressBarComponent } from './components/progress-bar/progress-bar.component';
import { ImageModule } from 'primeng/image';
import { ImageComponent } from './components/image/image.component';
import { DividerModule } from 'primeng/divider';
import { SplitButtonModule } from 'primeng/splitbutton';
import { SplitButtonComponent } from './components/split-button/split-button.component';
import { CustomMenuComponent } from './components/custom-menu/custom-menu.component';
import { MenuModule } from 'primeng/menu';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { SidebarModule } from 'primeng/sidebar';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { SelectButtonComponent } from './components/select-button/select-button.component';
import { TagComponent } from './components/tag/tag.component';
import { ChartModule } from 'primeng/chart';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { AutoCompleteComponent } from './components/auto-complete/autocomplete.component';
import { ProductSearchComponent } from './components/product-search/product-search.component';
import { InputDatepickerComponent } from './components/input-datepicker/input-datepicker.component';
import { PermissionDirectivesModule } from '../permission-directives/permission-directives.module';

@NgModule({
	declarations: [
		ConfirmDialogComponent,
		ToastComponent,
		DataGridComponent,
		InputTextComponent,
		ValidatorMsgComponent,
		InputTextAreaComponent,
		InputNumberComponent,
		InputCurrencyComponent,
		InputDecimalComponent,
		InputDateComponent,
		InputTimeComponent,
		InputYearComponent,
		InputSwitchComponent,
		InputCheckboxComponent,
		InputSelectComponent,
		InputRadioComponent,
		InputMultiselectComponent,
		InputColorComponent,
		InputFileComponent,
		InputFileAdvComponent,
		InputMaskComponent,
		InputPasswordComponent,
		InputEditorComponent,
		ButtonComponent,
		TreeComponent,
		TreeSelectComponent,
		ProgressBarComponent,
		ImageComponent,
		SplitButtonComponent,
		CustomMenuComponent,
		SidebarComponent,
		SelectButtonComponent,
		TagComponent,
		AutoCompleteComponent,
		ProductSearchComponent,
		InputDatepickerComponent
	],
	imports: [
		CommonModule,
		FormsModule,
		ReactiveFormsModule,
		PermissionDirectivesModule,
		//
		EditorModule,

		// PrimeNg Modules //
		ToastModule,
		ConfirmDialogModule,
		TableModule,
		ToolbarModule,
		FileUploadModule,
		DropdownModule,
		MultiSelectModule,
		TagModule,
		InputTextModule,
		InputTextareaModule,
		InputNumberModule,
		InputSwitchModule, 
		TooltipModule,
		CalendarModule,
		CheckboxModule ,
		RadioButtonModule,
		ColorPickerModule,
		InputMaskModule,
		PasswordModule,
		TreeModule ,
		TreeSelectModule,
		SelectButtonModule,
		OverlayPanelModule,
		ProgressBarModule,
		ImageModule,
		DividerModule,
		SplitButtonModule ,
		MenuModule,
		BadgeModule,
		AvatarModule,
		SidebarModule,
		ChartModule,
		AutoCompleteModule 

	],
	providers: [
		MessageService,
		ConfirmationService,
		DialogService,
		CustomDialogService,
		ToastService,
		// SidebarService
	],
	exports: [
		ConfirmDialogComponent,
		ToastComponent,
		DataGridComponent,
		InputTextComponent,
		ValidatorMsgComponent,
		InputTextAreaComponent,
		InputNumberComponent,
		InputCurrencyComponent,
		InputDecimalComponent,
		InputDateComponent,
		InputTimeComponent,
		InputYearComponent,
		InputSwitchComponent,
		InputCheckboxComponent,
		InputSelectComponent,
		InputRadioComponent,
		InputMultiselectComponent,
		InputColorComponent,
		InputFileComponent,
		InputFileAdvComponent,
		InputMaskComponent,
		InputPasswordComponent,
		InputEditorComponent,
		ButtonComponent,
		TreeComponent,
		TreeSelectComponent,
		ImageComponent,
		SplitButtonComponent,
		CustomMenuComponent,
		SidebarComponent,
		SelectButtonComponent,
		TagComponent,
		TableModule,
		AutoCompleteComponent,
		ProductSearchComponent,
		InputDatepickerComponent,

		PermissionDirectivesModule
	]
})
export class AppSharedModule { }
