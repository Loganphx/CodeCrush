import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
import { TabsModule } from "ngx-bootstrap/tabs";
import { ToastrModule } from "ngx-toastr";
import {NgxSpinnerModule} from "ngx-spinner";
import {FileUploadModule} from "ng2-file-upload";
import {BsDatepickerModule} from "ngx-bootstrap/datepicker";
import {MatPaginatorModule} from "@angular/material/paginator";
import {ModalModule} from "ngx-bootstrap/modal";

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: "toast-bottom-right"
    }),
    NgxSpinnerModule.forRoot({
      type: 'line-scale-party'
    }),
    BsDatepickerModule.forRoot(),
    MatPaginatorModule,
    FileUploadModule,
    ModalModule.forRoot(),
  ],
  exports: [
    BsDropdownModule,
    TabsModule,
    ToastrModule,
    NgxSpinnerModule,
    FileUploadModule,
    BsDatepickerModule,
    MatPaginatorModule,
    ModalModule,
  ]
})
export class SharedModule { }
