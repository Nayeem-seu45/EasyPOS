﻿namespace EasyPOS.Application.Features.HRM.LeaveRequests.Queries;

public record LeaveRequestModel
{
    public Guid Id { get; set; }
    public int TotalDays {get;set;} 
    public Guid EmployeeId {get;set;} 
    public Guid LeaveTypeId {get;set;} 
    public Guid StatusId {get;set;} 
    public string? AttachmentUrl {get;set;} 
    public string? Reason {get;set;} 


    public Dictionary<string, object> OptionsDataSources { get; set; } = [];
}

