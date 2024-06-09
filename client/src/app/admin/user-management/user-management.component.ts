import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../_services/admin.service";
import {User} from "../../_models/user";
import {BsModalRef, BsModalService, ModalOptions} from "ngx-bootstrap/modal";
import {RolesModalComponent} from "../../modals/roles-modal/roles-modal.component";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  availableRoles: string[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService) {
  }

  ngOnInit(): void {
    this.getUsersWithRoles();
    this.getAvailableRoles();
  }

  getUsersWithRoles(): void {
    this.adminService.getUserWithRoles().subscribe({
      next: users => this.users = users
    })
  }

  getAvailableRoles(): void {
    this.adminService.getAvailableRoles().subscribe({
      next: availableRoles => this.availableRoles = availableRoles
    })
  }

  openRolesModal(user: User): void {
    console.log(this.availableRoles)
    const config: ModalOptions = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles]
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config)
    // this.bsModalRef.content!.closeBtnName = 'Close via Content'
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content!.selectedRoles;
        if (!this.arrayEqual(selectedRoles, user.roles)) {
          this.adminService.updateUserRoles(user.username, selectedRoles.join(',')).subscribe({
            next: roles => user.roles = roles
          })
        }
      }
    })
  }

  private arrayEqual(array1: any[], array2: any[]): boolean {
    return JSON.stringify(array1.sort()) === JSON.stringify(array2.sort())
  }


}
