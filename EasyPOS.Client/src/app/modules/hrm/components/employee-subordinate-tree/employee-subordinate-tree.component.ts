import { Component, OnInit } from '@angular/core';
import { TreeNode } from 'primeng/api';
import { EmployeesClient } from 'src/app/modules/generated-clients/api-service';
import { CustomDialogService } from 'src/app/shared/services/custom-dialog.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CommonUtils } from 'src/app/shared/Utilities/common-utilities';

@Component({
  selector: 'app-employee-subordinate-tree',
  templateUrl: './employee-subordinate-tree.component.html',
  styleUrl: './employee-subordinate-tree.component.scss',
  providers: [EmployeesClient]

})
export class EmployeeSubordinateTreeComponent implements OnInit {
  selectedNodes!: TreeNode[];
  employeeId: string;

  employeeHierarchy: TreeNode[] = [];

  constructor(private customDialogService: CustomDialogService,
    private entityClient: EmployeesClient,
    private toast: ToastService,
  ) {


  }

  ngOnInit(): void {
    this.employeeId = this.customDialogService.getConfigData();

    this.getSubordinateTree(this.employeeId)

  }

  getSubordinateTree(employeeId: string) {
    this.entityClient.getEmployeeHierarchy(employeeId).subscribe({
      next: (res: any) => {
        console.log(res)
        this.employeeHierarchy.push(res);
        console.log(this.employeeHierarchy)
      },
      error: (error) => {
        this.toast.showError(CommonUtils.getErrorMessage(error));
      }
    });
  }

  data: TreeNode[] = [
    {
      expanded: true,
      type: 'person',
      data: {
        image: 'https://primefaces.org/cdn/primeng/images/demo/avatar/amyelsner.png',
        name: 'Amy Elsner',
        title: 'CEO'
      },
      children: [
        {
          expanded: true,
          type: 'person',
          data: {
            image: 'https://primefaces.org/cdn/primeng/images/demo/avatar/annafali.png',
            name: 'Anna Fali',
            title: 'CMO'
          },
          children: [
            {
              label: 'Sales'
            },
            {
              label: 'Marketing'
            }
          ]
        },
        {
          expanded: true,
          type: 'person',
          data: {
            image: 'https://primefaces.org/cdn/primeng/images/demo/avatar/stephenshaw.png',
            name: 'Stephen Shaw',
            title: 'CTO'
          },
          children: [
            {
              label: 'Development'
            },
            {
              label: 'UI/UX Design'
            }
          ]
        }
      ]
    }
  ];
}
