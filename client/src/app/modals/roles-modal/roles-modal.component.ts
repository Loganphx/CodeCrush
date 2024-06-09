import { Component } from '@angular/core';
import {BsModalRef} from "ngx-bootstrap/modal";

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.scss']
})
export class RolesModalComponent {
  username: string = '';
  availableRoles: string = '';
  selectedRoles: string[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit() {
  }

  updateChecked(checkedValue: string): void {
    const index = this.selectedRoles.indexOf(checkedValue);
    if(index === -1) {
      this.selectedRoles.push(checkedValue)
    } else {
      this.selectedRoles.splice(index, 1);
    }
  }
}
