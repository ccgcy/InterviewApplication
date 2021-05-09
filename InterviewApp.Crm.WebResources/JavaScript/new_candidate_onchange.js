function NoRightToWorkWarning(executionContext) {
    var noRightToWork = 100000004;
    var onHoldStatus = 100000004;

    var rightToWorkOptionSetValue = executionContext.getFormContext().data.entity.attributes.get("new_righttoworkinnz").getValue();

    if (rightToWorkOptionSetValue == noRightToWork) {
        executionContext.getFormContext().ui.setFormNotification(
            'Non-resident Employee needs to have a valid visa to work in NZ. Please Verify before process this application further.',
            "WARNING",
            "RightToWorkWarning"
        );

        executionContext.getFormContext().data.entity.attributes.get("statuscode").setValue(onHoldStatus);
    }
}
