import { FormControl, FormGroup } from '@angular/forms';

export function controlHasError(
  form: FormGroup,
  formControlName: string,
  error: string
): boolean {
  const formControl = form.get(formControlName) as FormControl;
  return (
    formControl.touched && formControl.hasError(error) && formControl.invalid
  );
}
