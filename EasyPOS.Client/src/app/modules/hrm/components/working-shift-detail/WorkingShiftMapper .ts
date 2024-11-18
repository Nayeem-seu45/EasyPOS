import { WorkingShiftModel, CreateWorkingShiftCommand, UpdateWorkingShiftCommand, WorkingShiftDetailModel } from "src/app/modules/generated-clients/api-service";

export class WorkingShiftMapper {
  static toCreateCommand(item: WorkingShiftModel): CreateWorkingShiftCommand {
    const command = new CreateWorkingShiftCommand();
    command.shiftName = item.shiftName;
    command.description = item.description;
    command.isActive = item.isActive;
    command.workingShiftDetails = item.workingShiftDetails?.map(WorkingShiftMapper.mapDetail);
    return command;
  }

  static toUpdateCommand(item: WorkingShiftModel): UpdateWorkingShiftCommand {
    const command = new UpdateWorkingShiftCommand();
    command.id = item.id;
    command.shiftName = item.shiftName;
    command.description = item.description;
    command.isActive = item.isActive;
    command.workingShiftDetails = item.workingShiftDetails?.map(WorkingShiftMapper.mapDetail);
    return command;
  }

  private static mapDetail(detail: WorkingShiftDetailModel): WorkingShiftDetailModel {
    const mappedDetail = new WorkingShiftDetailModel();
    mappedDetail.workingShiftId = detail.workingShiftId;
    mappedDetail.startTime = detail.startTime;
    mappedDetail.endTime = detail.endTime;
    mappedDetail.dayOfWeek = detail.dayOfWeek;
    mappedDetail.isWeekend = detail.isWeekend;
    return mappedDetail;
  }
}

