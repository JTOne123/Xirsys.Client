using System;

namespace Xirsys.Client.Utilities
{
    public static class ErrorMessages
    {
        public const String Internal = "internal";

        public const String NotFound = "not_found";
        public const String Unauthorized = "unauthorized";
        public const String BadName = "bad_name";
        public const String MalformedPartner = "malformed_partner";
        public const String InvalidAccount = "invalid_account";

        public const String EmailExists = "email_exists";
        public const String UserNameExists = "username_exists";
        public const String BadUserName = "bad_username";

        public const String BadSegment = "bad_segment";
        public const String BadDomainFormat = "bad_domain_format";
        public const String BadApplicationFormat = "bad_app_format";
        public const String BadRoomFormat = "bad_room_format";

        public const String PathExists = "path_exists";
        public const String DomainExists = "domain_exists";
        public const String ApplicationExists = "application_exists";
        public const String RoomExists = "room_exists";

        public const String NoNamespace = "no_namespace";
        public const String NoDomain = "no_domain";
        public const String NoApplication = "no_application";
        public const String NoRoom = "no_room";

        public const String UserNotAdmin = "user_not_admin";

        public const String RequireEmail = "require_email";
        public const String RequirePassword = "require_password";
        public const String BadPasswordFormat = "bad_password_format";

        public const String BadMeContext = "bad_me_context";

        public const String UnauthorizedDomain = "unauthorized_domain";
        public const String BadLayerPrefix = "bad_layer_prefix";

        public const String ValueMissing = "value_missing";

        public const String VersionMissing = "_ver_missing";
        public const String VersionConflict = "version_conflict";
        public const String Locked = "locked";


        // our own errors
        public const String Parsing = "parsing_error";
    }
}
