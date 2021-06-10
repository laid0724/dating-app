import {
  NG_VALUE_ACCESSOR,
  ControlValueAccessor,
  FormControl,
  FormControlDirective,
  ControlContainer,
} from '@angular/forms';
import {
  Component,
  forwardRef,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';

/*
  I find this to be the best and cleanest implementation of the control value accessor for native HTML form elements,
  solving the following problems when implementing it for a reactive form:
  - You do not need to pass the entire formControl via an input; using formControlName alone suffices
  - No longer need to use hacky methods to inject ngControl that might introduce circular DI just to get
    access to the control or the error states
  - You are accessing CVAs that the angular team already wrote for elements such as input/radio/select etc via formControlDirective

  // see: https://stackoverflow.com/questions/45755958/how-to-get-formcontrol-instance-from-controlvalueaccessor
  // see: https://stackoverflow.com/questions/45536108/access-valid-value-of-custom-form-control
  see: https://medium.com/angular-in-depth/dont-reinvent-the-wheel-when-implementing-controlvalueaccessor-a0ed4ad0fafd
*/

export const TEXT_INPUT_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TextInputComponent),
  multi: true,
};

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss'],
  providers: [TEXT_INPUT_VALUE_ACCESSOR],
})
export class TextInputComponent implements OnInit, ControlValueAccessor {
  @ViewChild(FormControlDirective, { static: true })
  formControlDirective: FormControlDirective;

  @Input()
  formControl: FormControl;
  @Input()
  formControlName: string;

  @Input() label: string;
  @Input() type = 'text';

  get control(): FormControl {
    return (this.formControl ||
      this.controlContainer.control.get(this.formControlName)) as FormControl;
  }

  constructor(private controlContainer: ControlContainer) {}

  ngOnInit(): void {}

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
