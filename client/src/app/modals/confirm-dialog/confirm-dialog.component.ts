import {Component, OnInit} from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  title: string = '';
  message: string  = '';
  btnOkText: string  = '';
  btnCancelText: string  = '';
  result: boolean = false;

  constructor(public bsModalRef: BsModalRef) {
  }

  ngOnInit(): void {
    }

  confirm(): void {
    this.result = true;
    this.bsModalRef.hide();
  }

  decline(): void {
    this.bsModalRef.hide()
  }
}
