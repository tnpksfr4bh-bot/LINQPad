public enum EnvironmentType
{
	LinuxEditor, LinuxPlayer,
	OSXEditor,   OSXPlayer,
	WindowsEditor, WindowsPlayer,
	IPhonePlayer, Android,
	WebGLPlayer,
	WSAPlayerX86, WSAPlayerX64, WSAPlayerARM,
	PSP2, PS4, XboxOne, tvOS, Switch
}

public enum PlatformType
{
	NL, // null / guest
	AP, // Apple Sign-in
	AG, // Apple Game Center
	GG, // Google Play Games
	FB, // Facebook
	LC, // LC
	OS, // OneStore
}

public enum StoreType
{
	None   = 0,
	Google = 1,
	Apple  = 2,
}

public enum StatusResult
{
	NONE              = 0,
	FAILED,
	SUCCESS,
	MAINTENANCE,
	TERMINATE,
	ACCESS_DENIED,
	WRONG_REQUEST,
	DATA_ERROR,
	INVALID_TOKEN,
	EXPIRED_TOKEN,
	INVALID_REQUEST,
	UNAUTHORIZED_CLIENT,
	SERVER_ERROR,
	NOT_AVAILABLE,
	EXCEPTION,
	DATA_NOT_FOUND,
	PLAYER_NOT_FOUND,
	ALREADY_USED,
	UNABLE_PROCESS,
	DATA_RESTORE_REQUIRED,
	EXPIRED,
	SESSION_CONFLICT,
	SESSION_EXPIRED,
	TRY_AGAIN,
	RESOURCE_NOT_FOUND,
	LIMIT_EXCEEDED,
	RESOURCE_IN_USE,
	INTERNAL_SERVER_ERROR,
	CONDITIONAL_CHECK_FAILED,
	ITEM_COLLECTION_SIZE_LIMIT_EXCEEDED,
	PROVISIONED_THROUGHPUT_EXCEEDED,
	REQUEST_LIMIT_EXCEEDED,
	TRANSACTION_CONFLICT,
}

public enum PolicyType
{
	UNDER_AGE,
	TERMS_OF_SERVICE,
	PRIVACY_USAGE,
}

public enum AuthLevel
{
	NOACCESS = 0,
	ADMIN    = 1, // all permissions
	MANAGER  = 2, // all management for assigned apps
	MEMBER   = 3, // view + permission-based for assigned apps
	USER     = 4, // view only for assigned apps
}

public enum PermissionLevel
{
	NOACCESS   = 0,
	READWRITE  = 1,
	READONLY   = 2,
}
