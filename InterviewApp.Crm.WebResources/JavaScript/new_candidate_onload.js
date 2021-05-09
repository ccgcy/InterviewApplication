function Form_onload(executionContext) {
    var applicationStatus = executionContext.getFormContext().getAttribute("statuscode").getValue();

    if (applicationStatus == 100000003) {
        executionContext.getFormContext().ui
            .setFormNotification("This application is open.", "INFO", "1");
    }
    if (applicationStatus == 100000004) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate is on hold at the moment.", "INFO", "1");
    }
    if (applicationStatus == 1) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate's application has been checked.", "INFO", "1");
    }
    if (applicationStatus == 100000001) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate is in interview stage now.", "INFO", "1");
    }
    if (applicationStatus == 100000000) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate has been shortlisted.", "INFO", "1");
    }
    if (applicationStatus == 2) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate has been rejected.", "INFO", "1");
    }
    if (applicationStatus == 100000002) {
        executionContext.getFormContext().ui
            .setFormNotification("This candidate has been hired.", "INFO", "1");
    }
}
