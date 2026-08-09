namespace OnTime.Domain.Enums;

public enum ErrorCode
{
    Unknown = 0,
    Unauthorized = 1,
    Forbidden = 2,
    NotFound = 3,
    BadRequest = 4,
    ValidationFailed = 5,
    InvalidCredentials = 6,
    EmailPendingVerification = 7,
    LockedOut = 8,
    InvalidToken = 9,
    EmailAlreadyExists = 10,
    RegistrationFailed = 11,
    ProfileNotFound = 12,
    ShopNotFound = 13,
    SlugUnavailable = 14,
    ImageNotFound = 15,
    ImageProcessingFailed = 16,
    ProfessionalRoleRequired = 17,
    ResendConfirmationFailed = 18,
    ResetPasswordFailed = 19,
    ForgotPasswordFailed = 20,
    ConfirmEmailFailed = 21,
    InvalidConfirmationParameters = 22,
    InvalidResetParameters = 23,
    EmailRequired = 24,
    FirstNameRequired = 25,
    LastNameRequired = 26,
    InvalidPhoneNumber = 27
}
