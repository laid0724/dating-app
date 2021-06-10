import { AbstractControl, ValidatorFn } from '@angular/forms';

export function matchValuesValidator(matchingControlName: string): ValidatorFn {
  return (control: AbstractControl) => {
    return control?.value ===
      control?.parent?.controls[matchingControlName].value
      ? null
      : { isNotMatching: true };
  };
}
