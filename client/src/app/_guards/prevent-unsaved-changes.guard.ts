import {CanDeactivateFn} from '@angular/router';
import {MemberEditComponent} from "../members/member-edit/member-edit.component";
import {ConfirmService} from "../_services/confirm.service";
import {inject} from "@angular/core";

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService = inject(ConfirmService);
  if (component.editMemberFormGroup.dirty) {
    // return confirm("Are you sure you want to continue? Any unsaved changes will be lost")

    return confirmService.confirm();
  }
  return true;
};
