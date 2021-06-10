import { Component, forwardRef, Input, ViewChild } from '@angular/core';
import {
  FormControl,
  ControlContainer,
  ControlValueAccessor,
  FormControlDirective,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

export const DATE_INPUT_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DateInputComponent),
  multi: true,
};

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.scss'],
  providers: [DATE_INPUT_VALUE_ACCESSOR],
})
export class DateInputComponent implements ControlValueAccessor {
  @ViewChild(FormControlDirective, { static: true })
  formControlDirective: FormControlDirective;

  @Input() formControl: FormControl;
  @Input() formControlName: string;

  @Input() label: string;
  @Input() maxDate: Date; // for setting age requirement, e.g., 18+ only

  bsConfig: Partial<BsDatepickerConfig>;

  get control(): FormControl {
    return (this.formControl ||
      this.controlContainer.control.get(this.formControlName)) as FormControl;
  }

  constructor(private controlContainer: ControlContainer) {
    this.bsConfig = {
      containerClass: 'theme-default',
      dateInputFormat: 'DD MMMM YYYY',
    };
  }

  writeValue(value: any): void {
    this.formControlDirective.valueAccessor.writeValue(value);
  }

  registerOnChange(fn: any): void {
    this.formControlDirective.valueAccessor.registerOnTouched(fn);
  }

  registerOnTouched(fn: any): void {
    this.formControlDirective.valueAccessor.registerOnChange(fn);
  }

  setDisabledState(isDisabled: boolean): void {
    this.formControlDirective.valueAccessor.setDisabledState(isDisabled);
  }
}
