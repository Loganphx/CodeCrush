import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../_services/admin.service";
import {Role, User} from "../../_models/user";
import {BsModalRef, BsModalService, ModalOptions} from "ngx-bootstrap/modal";
import {RolesModalComponent} from "../../modals/roles-modal/roles-modal.component";
import {Pagination} from "../../_models/pagination";
import {RoleParams, UserParams} from "../../_models/userParams";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  pagination: Pagination | undefined;
  userParams?: RoleParams;
  users: Role[] = [];
  availableRoles: string[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  orderByList = [
    {value: 'age', display: 'Age'},
    {value: 'name', display: 'Name'},
    {value: 'created', display: 'Created'},
    {value: 'lastActive', display: 'Last Active'}
  ]

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService) {
  }

  ngOnInit(): void {
    this.userParams = new RoleParams();
    this.getUsersWithRoles();
    this.getAvailableRoles();

    console.log(this.userParams)
    console.log(this.pagination)
  }

  getUsersWithRoles(): void {
    this.adminService.getUserWithRoles(this.userParams!).subscribe({
      next: users => {
        this.users = users.result!;
        this.pagination = users.pagination;
      }
    })
  }

  getAvailableRoles(): void {
    this.adminService.getAvailableRoles().subscribe({
      next: availableRoles => this.availableRoles = availableRoles
    })
  }

  openRolesModal(user: Role): void {
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

  resetFilters() {
    // if (this.user) {
      this.userParams = new RoleParams();
      this.getUsersWithRoles();
    // }
  }

  pageChanged(event: any) {
    console.log("pageChanged", event)
    if (!this.userParams) return;
    this.userParams.pageIndex = event.pageIndex + 1;
    this.userParams.pageSize = event.pageSize;
    this.getUsersWithRoles();
  }

  private arrayEqual(array1: any[], array2: any[]): boolean {
    return JSON.stringify(array1.sort()) === JSON.stringify(array2.sort())
  }


}
