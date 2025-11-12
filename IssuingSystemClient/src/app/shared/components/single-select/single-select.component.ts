import {Component, forwardRef, Input} from '@angular/core';
import {FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule} from '@angular/forms';
import {ISelectItem} from './models/single-select.model';
import {CommonModule} from '@angular/common';
import {SelectModule} from 'primeng/select';

@Component({
  selector: 'app-single-select',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SelectModule,
    FormsModule
  ],
  templateUrl: './single-select.component.html',
  styleUrl: './single-select.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SingleSelectComponent),
      multi: true,
    },
  ],
})
export class SingleSelectComponent {
  @Input({required: true}) label: string = '';
  @Input({required: true}) items: ISelectItem[] = [];
  @Input() placeholder: string = 'Selecione uma opção';
  @Input() required: boolean = false;
  @Input() id: string = `select-${Math.random().toString(36).substring(2, 9)}`;

  private _value: any;

  onChange: (value: any) => void = () => {
  };
  onTouched: () => void = () => {
  };
  disabled: boolean = false;

  get value(): any {
    return this._value;
  }

  set value(val: any) {
    if (val !== this._value) {
      this._value = val;
      this.onChange(val);
    }
  }

  writeValue(value: any): void {
    this._value = value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onModelChange(event: any): void {
    this.value = event;
    this.onTouched();
  }
}
