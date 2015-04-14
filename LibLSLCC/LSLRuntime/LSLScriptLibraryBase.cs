namespace LibLSLCC.LSLRuntime
{
    public abstract class LSLScriptLibraryBase
    {
        public readonly static LSL_Types.LSLInteger PSYS_PART_START_ALPHA = 2;
        public readonly static LSL_Types.LSLInteger PSYS_PART_END_ALPHA = 4;
        public readonly static LSL_Types.LSLInteger PSYS_PART_START_COLOR = 1;
        public readonly static LSL_Types.LSLInteger PSYS_PART_END_COLOR = 3;
        public readonly static LSL_Types.LSLInteger PSYS_PART_START_SCALE = 5;
        public readonly static LSL_Types.LSLInteger PSYS_PART_END_SCALE = 6;
        public readonly static LSL_Types.LSLInteger PSYS_PART_MAX_AGE = 7;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_MAX_AGE = 19;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_ACCEL = 8;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_ANGLE_BEGIN = 22;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_ANGLE_END = 23;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_BURST_PART_COUNT = 15;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_BURST_RADIUS = 16;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_BURST_RATE = 13;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_BURST_SPEED_MIN = 17;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_BURST_SPEED_MAX = 18;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_INNERANGLE = 10;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_OUTERANGLE = 11;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_OMEGA = 21;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_TARGET_KEY = 20;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_TEXTURE = 12;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN = 9;
        public readonly static LSL_Types.LSLInteger PSYS_PART_FLAGS = 0;
        public readonly static LSL_Types.LSLInteger PSYS_PART_BOUNCE_MASK = 0x004;
        public readonly static LSL_Types.LSLInteger PSYS_PART_EMISSIVE_MASK = 0x100;
        public readonly static LSL_Types.LSLInteger PSYS_PART_FOLLOW_SRC_MASK = 0x010;
        public readonly static LSL_Types.LSLInteger PSYS_PART_FOLLOW_VELOCITY_MASK = 0x020;
        public readonly static LSL_Types.LSLInteger PSYS_PART_INTERP_COLOR_MASK = 0x001;
        public readonly static LSL_Types.LSLInteger PSYS_PART_INTERP_SCALE_MASK = 0x002;
        public readonly static LSL_Types.LSLInteger PSYS_PART_TARGET_LINEAR_MASK = 0x080;
        public readonly static LSL_Types.LSLInteger PSYS_PART_TARGET_POS_MASK = 0x040;
        public readonly static LSL_Types.LSLInteger PSYS_PART_WIND_MASK = 0x008;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN_ANGLE = 0x04;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN_ANGLE_CONE = 0x08;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN_ANGLE_CONE_EMPTY = 0x10;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN_DROP = 0x01;
        public readonly static LSL_Types.LSLInteger PSYS_SRC_PATTERN_EXPLODE = 0x02;
        public readonly static LSL_Types.LSLInteger TRUE = 1;
        public readonly static LSL_Types.LSLInteger ACTIVE = 0x2;
        public readonly static LSL_Types.LSLInteger AGENT = 0x1;
        public readonly static LSL_Types.LSLInteger AGENT_ALWAYS_RUN = 0x1000;
        public readonly static LSL_Types.LSLInteger AGENT_ATTACHMENTS = 0x0002;
        public readonly static LSL_Types.LSLInteger AGENT_AUTOPILOT = 0x2000;
        public readonly static LSL_Types.LSLInteger AGENT_AWAY = 0x0040;
        public readonly static LSL_Types.LSLInteger AGENT_BUSY = 0x0800;
        public readonly static LSL_Types.LSLInteger AGENT_BY_LEGACY_NAME = 0x1;
        public readonly static LSL_Types.LSLInteger AGENT_BY_USERNAME = 0x10;
        public readonly static LSL_Types.LSLInteger AGENT_CROUCHING = 0x0400;
        public readonly static LSL_Types.LSLInteger AGENT_FLYING = 0x0001;
        public readonly static LSL_Types.LSLInteger AGENT_IN_AIR = 0x0100;
        public readonly static LSL_Types.LSLInteger AGENT_LIST_PARCEL = 1;
        public readonly static LSL_Types.LSLInteger AGENT_LIST_PARCEL_OWNER = 2;
        public readonly static LSL_Types.LSLInteger AGENT_LIST_REGION = 4;
        public readonly static LSL_Types.LSLInteger AGENT_MOUSELOOK = 0x0008;
        public readonly static LSL_Types.LSLInteger AGENT_ON_OBJECT = 0x0020;
        public readonly static LSL_Types.LSLInteger AGENT_SCRIPTED = 0x0004;
        public readonly static LSL_Types.LSLInteger AGENT_SITTING = 0x0010;
        public readonly static LSL_Types.LSLInteger AGENT_TYPING = 0x0200;
        public readonly static LSL_Types.LSLInteger AGENT_WALKING = 0x0080;
        public readonly static LSL_Types.LSLInteger ALL_SIDES = -1;
        public readonly static LSL_Types.LSLInteger ANIM_ON = 0x01;
        public readonly static LSL_Types.LSLInteger ATTACH_AVATAR_CENTER = 40;
        public readonly static LSL_Types.LSLInteger ATTACH_BACK = 9;
        public readonly static LSL_Types.LSLInteger ATTACH_BELLY = 28;
        public readonly static LSL_Types.LSLInteger ATTACH_CHEST = 1;
        public readonly static LSL_Types.LSLInteger ATTACH_CHIN = 12;
        public readonly static LSL_Types.LSLInteger ATTACH_HEAD = 2;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_BOTTOM = 37;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_BOTTOM_LEFT = 36;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_BOTTOM_RIGHT = 38;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_CENTER_1 = 35;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_CENTER_2 = 31;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_TOP_CENTER = 33;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_TOP_LEFT = 34;
        public readonly static LSL_Types.LSLInteger ATTACH_HUD_TOP_RIGHT = 32;
        public readonly static LSL_Types.LSLInteger ATTACH_LEAR = 13;
        public readonly static LSL_Types.LSLInteger ATTACH_LEFT_PEC = 29;
        public readonly static LSL_Types.LSLInteger ATTACH_RPEC = 29;
        public readonly static LSL_Types.LSLInteger ATTACH_LEYE = 15;
        public readonly static LSL_Types.LSLInteger ATTACH_LFOOT = 7;
        public readonly static LSL_Types.LSLInteger ATTACH_LHAND = 5;
        public readonly static LSL_Types.LSLInteger ATTACH_LHIP = 25;
        public readonly static LSL_Types.LSLInteger ATTACH_LLARM = 21;
        public readonly static LSL_Types.LSLInteger ATTACH_LLLEG = 27;
        public readonly static LSL_Types.LSLInteger ATTACH_LSHOULDER = 3;
        public readonly static LSL_Types.LSLInteger ATTACH_LUARM = 20;
        public readonly static LSL_Types.LSLInteger ATTACH_LULEG = 26;
        public readonly static LSL_Types.LSLInteger ATTACH_MOUTH = 11;
        public readonly static LSL_Types.LSLInteger ATTACH_NECK = 39;
        public readonly static LSL_Types.LSLInteger ATTACH_NOSE = 17;
        public readonly static LSL_Types.LSLInteger ATTACH_PELVIS = 10;
        public readonly static LSL_Types.LSLInteger ATTACH_REAR = 14;
        public readonly static LSL_Types.LSLInteger ATTACH_REYE = 16;
        public readonly static LSL_Types.LSLInteger ATTACH_RFOOT = 8;
        public readonly static LSL_Types.LSLInteger ATTACH_RHAND = 6;
        public readonly static LSL_Types.LSLInteger ATTACH_RHIP = 22;
        public readonly static LSL_Types.LSLInteger ATTACH_RIGHT_PEC = 30;
        public readonly static LSL_Types.LSLInteger ATTACH_LPEC = 30;
        public readonly static LSL_Types.LSLInteger ATTACH_RLARM = 19;
        public readonly static LSL_Types.LSLInteger ATTACH_RLLEG = 24;
        public readonly static LSL_Types.LSLInteger ATTACH_RSHOULDER = 4;
        public readonly static LSL_Types.LSLInteger ATTACH_RUARM = 18;
        public readonly static LSL_Types.LSLInteger ATTACH_RULEG = 23;
        public readonly static LSL_Types.LSLInteger CAMERA_ACTIVE = 12;
        public readonly static LSL_Types.LSLInteger CAMERA_BEHINDNESS_ANGLE = 8;
        public readonly static LSL_Types.LSLInteger CAMERA_BEHINDNESS_LAG = 9;
        public readonly static LSL_Types.LSLInteger CAMERA_DISTANCE = 7;
        public readonly static LSL_Types.LSLInteger CAMERA_FOCUS = 17;
        public readonly static LSL_Types.LSLInteger CAMERA_FOCUS_LAG = 6;
        public readonly static LSL_Types.LSLInteger CAMERA_FOCUS_LOCKED = 22;
        public readonly static LSL_Types.LSLInteger CAMERA_FOCUS_OFFSET = 1;
        public readonly static LSL_Types.LSLInteger CAMERA_FOCUS_THRESHOLD = 11;
        public readonly static LSL_Types.LSLInteger CAMERA_PITCH = 0;
        public readonly static LSL_Types.LSLInteger CAMERA_POSITION = 13;
        public readonly static LSL_Types.LSLInteger CAMERA_POSITION_LAG = 5;
        public readonly static LSL_Types.LSLInteger CAMERA_POSITION_LOCKED = 21;
        public readonly static LSL_Types.LSLInteger CAMERA_POSITION_THRESHOLD = 10;
        public readonly static LSL_Types.LSLInteger CHANGED_ALLOWED_DROP = 0x40;
        public readonly static LSL_Types.LSLInteger CHANGED_COLOR = 0x2;
        public readonly static LSL_Types.LSLInteger CHANGED_INVENTORY = 0x1;
        public readonly static LSL_Types.LSLInteger CHANGED_LINK = 0x20;
        public readonly static LSL_Types.LSLInteger CHANGED_MEDIA = 0x800;
        public readonly static LSL_Types.LSLInteger CHANGED_OWNER = 0x80;
        public readonly static LSL_Types.LSLInteger CHANGED_REGION = 0x100;
        public readonly static LSL_Types.LSLInteger CHANGED_REGION_START = 0x400;
        public readonly static LSL_Types.LSLInteger CHANGED_SCALE = 0x8;
        public readonly static LSL_Types.LSLInteger CHANGED_SHAPE = 0x4;
        public readonly static LSL_Types.LSLInteger CHANGED_TELEPORT = 0x200;
        public readonly static LSL_Types.LSLInteger CHANGED_TEXTURE = 0x10;
        public readonly static LSL_Types.LSLInteger CHARACTER_ACCOUNT_FOR_SKIPPED_FRAMES = 14;
        public readonly static LSL_Types.LSLInteger CHARACTER_AVOIDANCE_MODE = 5;
        public readonly static LSL_Types.LSLInteger CHARACTER_DESIRED_SPEED = 1;
        public readonly static LSL_Types.LSLInteger CHARACTER_DESIRED_TURN_SPEED = 12;
        public readonly static LSL_Types.LSLInteger CHARACTER_LENGTH = 3;
        public readonly static LSL_Types.LSLInteger CHARACTER_MAX_ACCEL = 8;
        public readonly static LSL_Types.LSLInteger CHARACTER_MAX_DECEL = 9;
        public readonly static LSL_Types.LSLInteger CHARACTER_MAX_SPEED = 13;
        public readonly static LSL_Types.LSLInteger CHARACTER_MAX_TURN_RADIUS = 10;
        public readonly static LSL_Types.LSLInteger CHARACTER_ORIENTATION = 4;
        public readonly static LSL_Types.LSLInteger CHARACTER_RADIUS = 2;
        public readonly static LSL_Types.LSLInteger CHARACTER_STAY_WITHIN_PARCEL = 15;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE = 6;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE_A = 0;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE_B = 1;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE_C = 2;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE_D = 3;
        public readonly static LSL_Types.LSLInteger CHARACTER_TYPE_NONE = 4;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_BUY = 2;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_NONE = 0;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_OPEN = 4;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_OPEN_MEDIA = 6;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_PAY = 3;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_PLAY = 5;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_SIT = 1;
        public readonly static LSL_Types.LSLInteger CLICK_ACTION_TOUCH = 0;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_ATOM = 4;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_FORM = 7;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_HTML = 1;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_JSON = 5;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_LLSD = 6;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_RSS = 8;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_TEXT = 0;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_XHTML = 3;
        public readonly static LSL_Types.LSLInteger CONTENT_TYPE_XML = 2;
        public readonly static LSL_Types.LSLInteger CONTROL_BACK = 0x2;
        public readonly static LSL_Types.LSLInteger CONTROL_DOWN = 0x20;
        public readonly static LSL_Types.LSLInteger CONTROL_FWD = 0x1;
        public readonly static LSL_Types.LSLInteger CONTROL_LBUTTON = 0x10000000;
        public readonly static LSL_Types.LSLInteger CONTROL_LEFT = 0x4;
        public readonly static LSL_Types.LSLInteger CONTROL_ML_LBUTTON = 0x40000000;
        public readonly static LSL_Types.LSLInteger CONTROL_RIGHT = 0x8;
        public readonly static LSL_Types.LSLInteger CONTROL_ROT_LEFT = 0x100;
        public readonly static LSL_Types.LSLInteger CONTROL_ROT_RIGHT = 0x200;
        public readonly static LSL_Types.LSLInteger CONTROL_UP = 0x10;
        public readonly static LSL_Types.LSLInteger DATA_BORN = 3;
        public readonly static LSL_Types.LSLInteger DATA_NAME = 2;
        public readonly static LSL_Types.LSLInteger DATA_ONLINE = 1;
        public readonly static LSL_Types.LSLInteger DATA_PAYINFO = 8;
        public readonly static LSL_Types.LSLInteger DATA_RATING = 4;
        public readonly static LSL_Types.LSLInteger DATA_SIM_POS = 5;
        public readonly static LSL_Types.LSLInteger DATA_SIM_RATING = 7;
        public readonly static LSL_Types.LSLInteger DATA_SIM_STATUS = 6;

        public readonly static LSL_Types.LSLInteger DEBUG_CHANNEL = 0x7FFFFFFF;
        //Chat channel reserved for script debugging and error messages, broadcasts to all nearby users.

        public readonly static LSL_Types.LSLFloat DEG_TO_RAD = 0.017453292519943295769236907684886f;
        public readonly static LSL_Types.LSLString EOF = "\n\n\n"; //Three newline characters (0x0a)
        public readonly static LSL_Types.LSLInteger ERR_GENERIC = -1;
        public readonly static LSL_Types.LSLInteger ERR_MALFORMED_PARAMS = -3;
        public readonly static LSL_Types.LSLInteger ERR_PARCEL_PERMISSIONS = -2;
        public readonly static LSL_Types.LSLInteger ERR_RUNTIME_PERMISSIONS = -4;
        public readonly static LSL_Types.LSLInteger ERR_THROTTLED = -5;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_ALLOWED_AGENT_ADD = 4;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_ALLOWED_AGENT_REMOVE = 8;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_ALLOWED_GROUP_ADD = 16;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_ALLOWED_GROUP_REMOVE = 32;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_BANNED_AGENT_ADD = 64;
        public readonly static LSL_Types.LSLInteger ESTATE_ACCESS_BANNED_AGENT_REMOVE = 128;
        public readonly static LSL_Types.LSLInteger FALSE = 0;
        public readonly static LSL_Types.LSLInteger FORCE_DIRECT_PATH = 1;
        public readonly static LSL_Types.LSLInteger HORIZONTAL = 1;
        public readonly static LSL_Types.LSLInteger HTTP_BODY_MAXLENGTH = 2;
        public readonly static LSL_Types.LSLInteger HTTP_BODY_TRUNCATED = 0;
        public readonly static LSL_Types.LSLInteger HTTP_CUSTOM_HEADER = 5;
        public readonly static LSL_Types.LSLInteger HTTP_METHOD = 0;
        public readonly static LSL_Types.LSLInteger HTTP_MIMETYPE = 1;
        public readonly static LSL_Types.LSLInteger HTTP_PRAGMA_NO_CACHE = 6;
        public readonly static LSL_Types.LSLInteger HTTP_VERBOSE_THROTTLE = 4;
        public readonly static LSL_Types.LSLInteger HTTP_VERIFY_CERT = 3;
        public readonly static LSL_Types.LSLInteger INVENTORY_ALL = -1;
        public readonly static LSL_Types.LSLInteger INVENTORY_ANIMATION = 20;
        public readonly static LSL_Types.LSLInteger INVENTORY_BODYPART = 13;
        public readonly static LSL_Types.LSLInteger INVENTORY_CLOTHING = 5;
        public readonly static LSL_Types.LSLInteger INVENTORY_GESTURE = 21;
        public readonly static LSL_Types.LSLInteger INVENTORY_LANDMARK = 3;
        public readonly static LSL_Types.LSLInteger INVENTORY_NONE = -1;
        public readonly static LSL_Types.LSLInteger INVENTORY_NOTECARD = 7;
        public readonly static LSL_Types.LSLInteger INVENTORY_OBJECT = 6;
        public readonly static LSL_Types.LSLInteger INVENTORY_SCRIPT = 10;
        public readonly static LSL_Types.LSLInteger INVENTORY_SOUND = 1;
        public readonly static LSL_Types.LSLInteger INVENTORY_TEXTURE = 0;
        public readonly static LSL_Types.LSLInteger JSON_APPEND = -1;
        public readonly static LSL_Types.LSLString JSON_ARRAY = "ï·’";
        public readonly static LSL_Types.LSLString JSON_DELETE = "ï¿½";
        public readonly static LSL_Types.LSLString JSON_FALSE = "ï¿½";
        public readonly static LSL_Types.LSLString JSON_INVALID = "ï·";
        public readonly static LSL_Types.LSLString JSON_NULL = "ï¿½";
        public readonly static LSL_Types.LSLString JSON_NUMBER = "ï¿½";
        public readonly static LSL_Types.LSLString JSON_OBJECT = "ï·‘";
        public readonly static LSL_Types.LSLString JSON_STRING = "ï¿½";
        public readonly static LSL_Types.LSLString JSON_TRUE = "ï¿½";
        public readonly static LSL_Types.LSLInteger KFM_CMD_PAUSE = 2;
        public readonly static LSL_Types.LSLInteger KFM_CMD_PLAY = 0;
        public readonly static LSL_Types.LSLInteger KFM_CMD_STOP = 1;
        public readonly static LSL_Types.LSLInteger KFM_COMMAND = 0;
        public readonly static LSL_Types.LSLInteger KFM_DATA = 2;
        public readonly static LSL_Types.LSLInteger KFM_FORWARD = 0;
        public readonly static LSL_Types.LSLInteger KFM_LOOP = 1;
        public readonly static LSL_Types.LSLInteger KFM_MODE = 1;
        public readonly static LSL_Types.LSLInteger KFM_PING_PONG = 2;
        public readonly static LSL_Types.LSLInteger KFM_REVERSE = 3;
        public readonly static LSL_Types.LSLInteger KFM_ROTATION = 0x1;
        public readonly static LSL_Types.LSLInteger KFM_TRANSLATION = 0x2;
        public readonly static LSL_Types.LSLInteger LAND_LEVEL = 0;
        public readonly static LSL_Types.LSLInteger LAND_LOWER = 2;
        public readonly static LSL_Types.LSLInteger LAND_NOISE = 4;
        public readonly static LSL_Types.LSLInteger LAND_RAISE = 1;
        public readonly static LSL_Types.LSLInteger LAND_REVERT = 5;
        public readonly static LSL_Types.LSLInteger LAND_SMOOTH = 3;
        public readonly static LSL_Types.LSLInteger LINK_ALL_CHILDREN = -3;
        public readonly static LSL_Types.LSLInteger LINK_ALL_OTHERS = -2;
        public readonly static LSL_Types.LSLInteger LINK_ROOT = 1;
        public readonly static LSL_Types.LSLInteger LINK_SET = -1;
        public readonly static LSL_Types.LSLInteger LINK_THIS = -4;
        public readonly static LSL_Types.LSLInteger LIST_STAT_GEOMETRIC_MEAN = 9;
        public readonly static LSL_Types.LSLInteger LIST_STAT_MAX = 2;
        public readonly static LSL_Types.LSLInteger LIST_STAT_MEAN = 3;
        public readonly static LSL_Types.LSLInteger LIST_STAT_MEDIAN = 4;
        public readonly static LSL_Types.LSLInteger LIST_STAT_MIN = 1;
        public readonly static LSL_Types.LSLInteger LIST_STAT_NUM_COUNT = 8;
        public readonly static LSL_Types.LSLInteger LIST_STAT_RANGE = 0;
        public readonly static LSL_Types.LSLInteger LIST_STAT_STD_DEV = 5;
        public readonly static LSL_Types.LSLInteger LIST_STAT_SUM = 6;
        public readonly static LSL_Types.LSLInteger LIST_STAT_SUM_SQUARES = 7;
        public readonly static LSL_Types.LSLInteger LOOP = 0x02;
        public readonly static LSL_Types.LSLInteger MASK_BASE = 0;
        public readonly static LSL_Types.LSLInteger MASK_EVERYONE = 3;
        public readonly static LSL_Types.LSLInteger MASK_GROUP = 2;
        public readonly static LSL_Types.LSLInteger MASK_NEXT = 4;
        public readonly static LSL_Types.LSLInteger MASK_OWNER = 1;
        public readonly static LSL_Types.LSLString NULL_KEY = "00000000-0000-0000-0000-000000000000";
        public readonly static LSL_Types.LSLInteger OBJECT_ATTACHED_POINT = 19;
        public readonly static LSL_Types.LSLInteger OBJECT_CHARACTER_TIME = 17;
        public readonly static LSL_Types.LSLInteger OBJECT_CREATOR = 8;
        public readonly static LSL_Types.LSLInteger OBJECT_DESC = 2;
        public readonly static LSL_Types.LSLInteger OBJECT_GROUP = 7;
        public readonly static LSL_Types.LSLInteger OBJECT_NAME = 1;
        public readonly static LSL_Types.LSLInteger OBJECT_OWNER = 6;
        public readonly static LSL_Types.LSLInteger OBJECT_PATHFINDING_TYPE = 20;
        public readonly static LSL_Types.LSLInteger OBJECT_PHANTOM = 22;
        public readonly static LSL_Types.LSLInteger OBJECT_PHYSICS = 21;
        public readonly static LSL_Types.LSLInteger OBJECT_PHYSICS_COST = 16;
        public readonly static LSL_Types.LSLInteger OBJECT_POS = 3;
        public readonly static LSL_Types.LSLInteger OBJECT_PRIM_EQUIVALENCE = 13;
        public readonly static LSL_Types.LSLInteger OBJECT_RENDER_WEIGHT = 24;
        public readonly static LSL_Types.LSLInteger OBJECT_RETURN_PARCEL = 1;
        public readonly static LSL_Types.LSLInteger OBJECT_RETURN_PARCEL_OWNER = 2;
        public readonly static LSL_Types.LSLInteger OBJECT_RETURN_REGION = 4;
        public readonly static LSL_Types.LSLInteger OBJECT_ROOT = 18;
        public readonly static LSL_Types.LSLInteger OBJECT_ROT = 4;
        public readonly static LSL_Types.LSLInteger OBJECT_RUNNING_SCRIPT_COUNT = 9;
        public readonly static LSL_Types.LSLInteger OBJECT_SCRIPT_MEMORY = 11;
        public readonly static LSL_Types.LSLInteger OBJECT_SCRIPT_TIME = 12;
        public readonly static LSL_Types.LSLInteger OBJECT_SERVER_COST = 14;
        public readonly static LSL_Types.LSLInteger OBJECT_STREAMING_COST = 15;
        public readonly static LSL_Types.LSLInteger OBJECT_TEMP_ON_REZ = 23;
        public readonly static LSL_Types.LSLInteger OBJECT_TOTAL_SCRIPT_COUNT = 10;
        public readonly static LSL_Types.LSLInteger OBJECT_UNKNOWN_DETAIL = -1;
        public readonly static LSL_Types.LSLInteger OBJECT_VELOCITY = 5;
        public readonly static LSL_Types.LSLInteger OPT_AVATAR = 1;
        public readonly static LSL_Types.LSLInteger OPT_CHARACTER = 2;
        public readonly static LSL_Types.LSLInteger OPT_EXCLUSION_VOLUME = 6;
        public readonly static LSL_Types.LSLInteger OPT_LEGACY_LINKSET = 0;
        public readonly static LSL_Types.LSLInteger OPT_MATERIAL_VOLUME = 5;
        public readonly static LSL_Types.LSLInteger OPT_OTHER = -1;
        public readonly static LSL_Types.LSLInteger OPT_STATIC_OBSTACLE = 4;
        public readonly static LSL_Types.LSLInteger OPT_WALKABLE = 3;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_GROUP = 2;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_OTHER = 3;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_OWNER = 1;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_SELECTED = 4;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_TEMP = 5;
        public readonly static LSL_Types.LSLInteger PARCEL_COUNT_TOTAL = 0;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_AREA = 4;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_DESC = 1;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_GROUP = 3;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_ID = 5;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_NAME = 0;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_OWNER = 2;
        public readonly static LSL_Types.LSLInteger PARCEL_DETAILS_SEE_AVATARS = 6;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_ALL_OBJECT_ENTRY = 0x8000000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_CREATE_GROUP_OBJECTS = 0x4000000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_CREATE_OBJECTS = 0x40;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_DAMAGE = 0x20;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_FLY = 0x1;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_GROUP_OBJECT_ENTRY = 0x10000000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_GROUP_SCRIPTS = 0x2000000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_LANDMARK = 0x8;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_SCRIPTS = 0x2;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_ALLOW_TERRAFORM = 0x10;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_LOCAL_SOUND_ONLY = 0x8000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_RESTRICT_PUSHOBJECT = 0x200000;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_USE_ACCESS_GROUP = 0x100;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_USE_ACCESS_LIST = 0x200;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_USE_BAN_LIST = 0x400;
        public readonly static LSL_Types.LSLInteger PARCEL_FLAG_USE_LAND_PASS_LIST = 0x800;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_AGENT = 7;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_AUTO_ALIGN = 9;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_DESC = 12;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_LOOP = 3;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_LOOP_SET = 13;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_PAUSE = 1;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_PLAY = 2;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_SIZE = 11;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_STOP = 0;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_TEXTURE = 4;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_TIME = 6;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_TYPE = 10;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_UNLOAD = 8;
        public readonly static LSL_Types.LSLInteger PARCEL_MEDIA_COMMAND_URL = 5;
        public readonly static LSL_Types.LSLInteger PASSIVE = 0x4;
        public readonly static LSL_Types.LSLInteger PATROL_PAUSE_AT_WAYPOINTS = 0;
        public readonly static LSL_Types.LSLInteger PAYMENT_INFO_ON_FILE = 0x1; //If payment info is on file.
        public readonly static LSL_Types.LSLInteger PAYMENT_INFO_USED = 0x2; //If payment info has been used.
        public readonly static LSL_Types.LSLInteger PAY_DEFAULT = -2;
        public readonly static LSL_Types.LSLInteger PAY_HIDE = -1;
        public readonly static LSL_Types.LSLInteger PERMISSION_ATTACH = 0x20;
        public readonly static LSL_Types.LSLInteger PERMISSION_CHANGE_LINKS = 0x80;
        public readonly static LSL_Types.LSLInteger PERMISSION_CONTROL_CAMERA = 0x800;
        public readonly static LSL_Types.LSLInteger PERMISSION_DEBIT = 0x2;
        public readonly static LSL_Types.LSLInteger PERMISSION_OVERRIDE_ANIMATIONS = 0x8000;
        public readonly static LSL_Types.LSLInteger PERMISSION_RETURN_OBJECTS = 65536;
        public readonly static LSL_Types.LSLInteger PERMISSION_SILENT_ESTATE_MANAGEMENT = 0x4000;
        public readonly static LSL_Types.LSLInteger PERMISSION_TAKE_CONTROLS = 0x4;
        public readonly static LSL_Types.LSLInteger PERMISSION_TELEPORT = 0x1000;
        public readonly static LSL_Types.LSLInteger PERMISSION_TRACK_CAMERA = 0x400;
        public readonly static LSL_Types.LSLInteger PERMISSION_TRIGGER_ANIMATION = 0x10;
        public readonly static LSL_Types.LSLInteger PERM_ALL = 0x7FFFFFFF;
        public readonly static LSL_Types.LSLInteger PERM_COPY = 0x00008000;
        public readonly static LSL_Types.LSLInteger PERM_MODIFY = 0x00004000;
        public readonly static LSL_Types.LSLInteger PERM_MOVE = 0x00080000;
        public readonly static LSL_Types.LSLInteger PERM_TRANSFER = 0x00002000;
        public readonly static LSL_Types.LSLFloat PI = 3.1415926535897932384626433832795f;
        public readonly static LSL_Types.LSLInteger PING_PONG = 0x08;
        public readonly static LSL_Types.LSLFloat PI_BY_TWO = 1.5707963267948966192313216916398f;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_BARK = 4;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_BLOBS = 12;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_BRICKS = 5;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_BRIGHT = 1;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_CHECKER = 6;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_CONCRETE = 7;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_DARK = 2;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_DISKS = 10;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_GRAVEL = 11;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_LARGETILE = 14;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_NONE = 0;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_SHINY = 19;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_SIDING = 13;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_STONE = 9;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_STUCCO = 15;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_SUCTION = 16;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_TILE = 8;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_WEAVE = 17;
        public readonly static LSL_Types.LSLInteger PRIM_BUMP_WOOD = 3;
        public readonly static LSL_Types.LSLInteger PRIM_COLOR = 18;
        public readonly static LSL_Types.LSLInteger PRIM_DESC = 28;
        public readonly static LSL_Types.LSLInteger PRIM_FLEXIBLE = 21;
        public readonly static LSL_Types.LSLInteger PRIM_FULLBRIGHT = 20;
        public readonly static LSL_Types.LSLInteger PRIM_GLOW = 25;
        public readonly static LSL_Types.LSLInteger PRIM_HOLE_CIRCLE = 0x10;
        public readonly static LSL_Types.LSLInteger PRIM_HOLE_DEFAULT = 0x00;
        public readonly static LSL_Types.LSLInteger PRIM_HOLE_SQUARE = 0x20;
        public readonly static LSL_Types.LSLInteger PRIM_HOLE_TRIANGLE = 0x30;
        public readonly static LSL_Types.LSLInteger PRIM_LINK_TARGET = 34;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL = 2;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_FLESH = 4;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_GLASS = 2;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_LIGHT = 7;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_METAL = 1;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_PLASTIC = 5;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_RUBBER = 6;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_STONE = 0;
        public readonly static LSL_Types.LSLInteger PRIM_MATERIAL_WOOD = 3;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_ALT_IMAGE_ENABLE = 0;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_AUTO_LOOP = 4;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_AUTO_PLAY = 5;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_AUTO_SCALE = 6;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_AUTO_ZOOM = 7;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_CURRENT_URL = 2;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_FIRST_CLICK_INTERACT = 8;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_HEIGHT_PIXELS = 10;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_HOME_URL = 3;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERMS_CONTROL = 14;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERMS_INTERACT = 13;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERM_ANYONE = 0x4;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERM_GROUP = 0x2;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERM_NONE = 0x0;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_PERM_OWNER = 0x1;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_WHITELIST = 12;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_WHITELIST_ENABLE = 11;
        public readonly static LSL_Types.LSLInteger PRIM_MEDIA_WIDTH_PIXELS = 9;
        public readonly static LSL_Types.LSLInteger PRIM_NAME = 27;
        public readonly static LSL_Types.LSLInteger PRIM_OMEGA = 32;
        public readonly static LSL_Types.LSLInteger PRIM_PHANTOM = 5;
        public readonly static LSL_Types.LSLInteger PRIM_PHYSICS = 3;
        public readonly static LSL_Types.LSLInteger PRIM_PHYSICS_SHAPE_CONVEX = 2;
        public readonly static LSL_Types.LSLInteger PRIM_PHYSICS_SHAPE_NONE = 1;
        public readonly static LSL_Types.LSLInteger PRIM_PHYSICS_SHAPE_PRIM = 0;
        public readonly static LSL_Types.LSLInteger PRIM_PHYSICS_SHAPE_TYPE = 30;
        public readonly static LSL_Types.LSLInteger PRIM_POINT_LIGHT = 23;
        public readonly static LSL_Types.LSLInteger PRIM_POSITION = 6;
        public readonly static LSL_Types.LSLInteger PRIM_POS_LOCAL = 33;
        public readonly static LSL_Types.LSLInteger PRIM_ROTATION = 8;
        public readonly static LSL_Types.LSLInteger PRIM_ROT_LOCAL = 29;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_FLAG_INVERT = 0x40;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_FLAG_MIRROR = 0x80;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_TYPE_CYLINDER = 4;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_TYPE_MASK = 7;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_TYPE_PLANE = 3;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_TYPE_SPHERE = 1;
        public readonly static LSL_Types.LSLInteger PRIM_SCULPT_TYPE_TORUS = 2;
        public readonly static LSL_Types.LSLInteger PRIM_SHINY_HIGH = 3;
        public readonly static LSL_Types.LSLInteger PRIM_SHINY_LOW = 1;
        public readonly static LSL_Types.LSLInteger PRIM_SHINY_MEDIUM = 2;
        public readonly static LSL_Types.LSLInteger PRIM_SHINY_NONE = 0;
        public readonly static LSL_Types.LSLInteger PRIM_SIZE = 7;
        public readonly static LSL_Types.LSLInteger PRIM_SLICE = 35;
        public readonly static LSL_Types.LSLInteger PRIM_TEMP_ON_REZ = 4;
        public readonly static LSL_Types.LSLInteger PRIM_TEXGEN = 22;
        public readonly static LSL_Types.LSLInteger PRIM_TEXGEN_DEFAULT = 0;
        public readonly static LSL_Types.LSLInteger PRIM_TEXGEN_PLANAR = 1;
        public readonly static LSL_Types.LSLInteger PRIM_TEXT = 26;
        public readonly static LSL_Types.LSLInteger PRIM_TEXTURE = 17;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE = 9;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_BOX = 0;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_CYLINDER = 1;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_PRISM = 2;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_RING = 6;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_SCULPT = 7;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_SPHERE = 3;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_TORUS = 4;
        public readonly static LSL_Types.LSLInteger PRIM_TYPE_TUBE = 5;
        public readonly static LSL_Types.LSLInteger PROFILE_NONE = 0;
        public readonly static LSL_Types.LSLInteger PROFILE_SCRIPT_MEMORY = 1;

        public readonly static LSL_Types.LSLInteger PUBLIC_CHANNEL = 0x0;


        public readonly static LSL_Types.LSLFloat RAD_TO_DEG = 57.295779513082320876798154814105f;
        public readonly static LSL_Types.LSLInteger RCERR_CAST_TIME_EXCEEDED = -3;
        public readonly static LSL_Types.LSLInteger RCERR_SIM_PERF_LOW = -2;
        public readonly static LSL_Types.LSLInteger RCERR_UNKNOWN = -1;
        public readonly static LSL_Types.LSLInteger RC_DATA_FLAGS = 2;
        public readonly static LSL_Types.LSLInteger RC_DETECT_PHANTOM = 1;
        public readonly static LSL_Types.LSLInteger RC_GET_LINK_NUM = 4;
        public readonly static LSL_Types.LSLInteger RC_GET_NORMAL = 1;
        public readonly static LSL_Types.LSLInteger RC_GET_ROOT_KEY = 2;
        public readonly static LSL_Types.LSLInteger RC_MAX_HITS = 3;
        public readonly static LSL_Types.LSLInteger RC_REJECT_AGENTS = 1;
        public readonly static LSL_Types.LSLInteger RC_REJECT_LAND = 8;
        public readonly static LSL_Types.LSLInteger RC_REJECT_NONPHYSICAL = 4;
        public readonly static LSL_Types.LSLInteger RC_REJECT_PHYSICAL = 2;
        public readonly static LSL_Types.LSLInteger RC_REJECT_TYPES = 0;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_ALLOW_DAMAGE = 0x00000001;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_ALLOW_DIRECT_TELEPORT = 0x00100000;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_BLOCK_FLY = 0x00080000;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_BLOCK_TERRAFORM = 0x00000040;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_DISABLE_COLLISIONS = 0x00001000;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_DISABLE_PHYSICS = 0x00004000;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_FIXED_SUN = 0x00000010;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_RESTRICT_PUSHOBJECT = 0x00400000;
        public readonly static LSL_Types.LSLInteger REGION_FLAG_SANDBOX = 0x00000100;
        public readonly static LSL_Types.LSLInteger REMOTE_DATA_CHANNEL = 1;
        public readonly static LSL_Types.LSLInteger REMOTE_DATA_REPLY = 3;
        public readonly static LSL_Types.LSLInteger REMOTE_DATA_REQUEST = 2;
        public readonly static LSL_Types.LSLInteger REVERSE = 0x04;
        public readonly static LSL_Types.LSLInteger ROTATE = 0x20;
        public readonly static LSL_Types.LSLInteger SCALE = 0x40;
        public readonly static LSL_Types.LSLInteger SCRIPTED = 0x8;
        public readonly static LSL_Types.LSLInteger SIM_STAT_PCT_CHARS_STEPPED = 0;
        public readonly static LSL_Types.LSLInteger SMOOTH = 0x010;
        public readonly static LSL_Types.LSLFloat SQRT2 = 1.4142135623730950488016887242097f;
        public readonly static LSL_Types.LSLInteger STATUS_BLOCK_GRAB = 0x40;
        public readonly static LSL_Types.LSLInteger STATUS_BLOCK_GRAB_OBJECT = 0x400; //test
        public readonly static LSL_Types.LSLInteger STATUS_BOUNDS_ERROR = 1002;
        public readonly static LSL_Types.LSLInteger STATUS_CAST_SHADOWS = 0x200;
        public readonly static LSL_Types.LSLInteger STATUS_DIE_AT_EDGE = 0x80;
        public readonly static LSL_Types.LSLInteger STATUS_INTERNAL_ERROR = 1999;
        public readonly static LSL_Types.LSLInteger STATUS_MALFORMED_PARAMS = 1000;
        public readonly static LSL_Types.LSLInteger STATUS_NOT_FOUND = 1003;
        public readonly static LSL_Types.LSLInteger STATUS_NOT_SUPPORTED = 1004;
        public readonly static LSL_Types.LSLInteger STATUS_OK = 0;
        public readonly static LSL_Types.LSLInteger STATUS_PHANTOM = 0x10;
        public readonly static LSL_Types.LSLInteger STATUS_PHYSICS = 0x1;
        public readonly static LSL_Types.LSLInteger STATUS_RETURN_AT_EDGE = 0x100;
        public readonly static LSL_Types.LSLInteger STATUS_ROTATE_X = 0x2;
        public readonly static LSL_Types.LSLInteger STATUS_ROTATE_Y = 0x4;
        public readonly static LSL_Types.LSLInteger STATUS_ROTATE_Z = 0x8;
        public readonly static LSL_Types.LSLInteger STATUS_SANDBOX = 0x20;
        public readonly static LSL_Types.LSLInteger STATUS_TYPE_MISMATCH = 1001;
        public readonly static LSL_Types.LSLInteger STATUS_WHITELIST_FAILED = 2001;
        public readonly static LSL_Types.LSLInteger STRING_TRIM = 0x03;
        public readonly static LSL_Types.LSLInteger STRING_TRIM_HEAD = 0x01;
        public readonly static LSL_Types.LSLInteger STRING_TRIM_TAIL = 0x02;
        public readonly static LSL_Types.LSLString TEXTURE_BLANK = "5748decc-f629-461c-9a36-a35a221fe21f";
        public readonly static LSL_Types.LSLString TEXTURE_DEFAULT = "89556747-24cb-43ed-920b-47caed15465f";
        public readonly static LSL_Types.LSLString TEXTURE_MEDIA = "8b5fec65-8d8d-9dc5-cda8-8fdf2716e361";
        public readonly static LSL_Types.LSLString TEXTURE_PLYWOOD = "89556747-24cb-43ed-920b-47caed15465f";
        public readonly static LSL_Types.LSLString TEXTURE_TRANSPARENT = "8dcd4a48-2d37-4909-9f78-f7a9eb4ef903";
        public readonly static LSL_Types.LSLInteger TOUCH_INVALID_FACE = 0xFFFFFFFF;
        public readonly static LSL_Types.Vector3 TOUCH_INVALID_TEXCOORD = new LSL_Types.Vector3(-1.0, -1.0, 0.0);
        public readonly static LSL_Types.Vector3 TOUCH_INVALID_VECTOR = new LSL_Types.Vector3(0.0, 0.0, 0.0);
        public readonly static LSL_Types.LSLInteger TRAVERSAL_TYPE = 7;
        public readonly static LSL_Types.LSLFloat TWO_PI = 6.283185307179586476925286766559f;
        public readonly static LSL_Types.LSLInteger TYPE_FLOAT = 2;
        public readonly static LSL_Types.LSLInteger TYPE_INTEGER = 1;
        public readonly static LSL_Types.LSLInteger TYPE_INVALID = 0;
        public readonly static LSL_Types.LSLInteger TYPE_KEY = 4;
        public readonly static LSL_Types.LSLInteger TYPE_ROTATION = 6;
        public readonly static LSL_Types.LSLInteger TYPE_STRING = 3;
        public readonly static LSL_Types.LSLInteger TYPE_VECTOR = 5;
        public readonly static LSL_Types.LSLString URL_REQUEST_DENIED = "URL_REQUEST_DENIED";
        public readonly static LSL_Types.LSLString URL_REQUEST_GRANTED = "URL_REQUEST_GRANTED";
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_DEFLECTION_EFFICIENCY = 32;
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_DEFLECTION_TIMESCALE = 33;
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_FRICTION_TIMESCALE = 17;
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_MOTOR_DECAY_TIMESCALE = 35;
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_MOTOR_DIRECTION = 19;
        public readonly static LSL_Types.LSLInteger VEHICLE_ANGULAR_MOTOR_TIMESCALE = 34;
        public readonly static LSL_Types.LSLInteger VEHICLE_BANKING_EFFICIENCY = 38;
        public readonly static LSL_Types.LSLInteger VEHICLE_BANKING_MIX = 39;
        public readonly static LSL_Types.LSLInteger VEHICLE_BANKING_TIMESCALE = 40;
        public readonly static LSL_Types.LSLInteger VEHICLE_BUOYANCY = 27;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_CAMERA_DECOUPLED = 0x200;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_HOVER_GLOBAL_HEIGHT = 0x010;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_HOVER_TERRAIN_ONLY = 0x008;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_HOVER_UP_ONLY = 0x020;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_HOVER_WATER_ONLY = 0x004;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_LIMIT_MOTOR_UP = 0x080;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_LIMIT_ROLL_ONLY = 0x002;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_MOUSELOOK_BANK = 0x200;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_MOUSELOOK_STEER = 0x100;
        public readonly static LSL_Types.LSLInteger VEHICLE_FLAG_NO_DEFLECTION_UP = 0x001;
        public readonly static LSL_Types.LSLInteger VEHICLE_HOVER_EFFICIENCY = 25;
        public readonly static LSL_Types.LSLInteger VEHICLE_HOVER_HEIGHT = 24;
        public readonly static LSL_Types.LSLInteger VEHICLE_HOVER_TIMESCALE = 26;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_DEFLECTION_EFFICIENCY = 28;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_DEFLECTION_TIMESCALE = 31;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_FRICTION_TIMESCALE = 16;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_MOTOR_DECAY_TIMESCALE = 31;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_MOTOR_DIRECTION = 18;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_MOTOR_OFFSET = 20;
        public readonly static LSL_Types.LSLInteger VEHICLE_LINEAR_MOTOR_TIMESCALE = 30;
        public readonly static LSL_Types.LSLInteger VEHICLE_REFERENCE_FRAME = 44;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_AIRPLANE = 4;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_BALLOON = 5;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_BOAT = 3;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_CAR = 2;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_NONE = 0;
        public readonly static LSL_Types.LSLInteger VEHICLE_TYPE_SLED = 1;
        public readonly static LSL_Types.LSLInteger VEHICLE_VERTICAL_ATTRACTION_EFFICIENCY = 36;
        public readonly static LSL_Types.LSLInteger VEHICLE_VERTICAL_ATTRACTION_TIMESCALE = 37;
        public readonly static LSL_Types.LSLInteger VERTICAL = 0;
        public readonly static LSL_Types.Quaternion ZERO_ROTATION = new LSL_Types.Quaternion(0.0, 0.0, 0.0, 1.0);
        public readonly static LSL_Types.Vector3    ZERO_VECTOR = new LSL_Types.Vector3(0.0, 0.0, 0.0);


        public abstract void at_target(LSL_Types.LSLInteger tnum, LSL_Types.Vector3 targetpos, LSL_Types.Vector3 ourpos);

        public abstract void control(LSL_Types.key id, LSL_Types.LSLInteger level, LSL_Types.LSLInteger edge);


        public abstract void at_rot_target(LSL_Types.LSLInteger handle, LSL_Types.Quaternion targetrot,
            LSL_Types.Quaternion ourrot);


        public abstract void not_at_rot_target();

        public abstract void not_at_target();


        public abstract void http_response(LSL_Types.key request_id, LSL_Types.LSLInteger status,
            LSL_Types.list metadata, LSL_Types.LSLString body);


        public abstract void on_rez(LSL_Types.LSLInteger start_param);

        public abstract void attach(LSL_Types.key id);

        public abstract void land_collision(LSL_Types.Vector3 pos);

        public abstract void path_update(LSL_Types.LSLInteger type, LSL_Types.list reserved);

        public abstract void changed(LSL_Types.LSLInteger change);

        public abstract void collision(LSL_Types.LSLInteger num_detected);

        public abstract void collision_start(LSL_Types.LSLInteger num_detected);

        public abstract void collision_end(LSL_Types.LSLInteger num_detected);

        public abstract void land_collision_end(LSL_Types.Vector3 pos);

        public abstract void land_collision_start(LSL_Types.Vector3 pos);


        public abstract void link_message(LSL_Types.LSLInteger sender_num, LSL_Types.LSLInteger num,
            LSL_Types.LSLString str, LSL_Types.key id);


        public abstract void listen(LSL_Types.LSLInteger channel, LSL_Types.LSLString name, LSL_Types.key id,
            LSL_Types.LSLString message);


        public abstract void remote_data(LSL_Types.LSLInteger event_type, LSL_Types.key channel,
            LSL_Types.key message_id, LSL_Types.LSLString sender, LSL_Types.LSLInteger idata, LSL_Types.LSLString sdata);


        public abstract void run_time_permissions(LSL_Types.LSLInteger perm);

        public abstract void sensor(LSL_Types.LSLInteger num_detected);

        public abstract void state_entry();

        public abstract void timer();

        public abstract void touch(LSL_Types.LSLInteger num_detected);

        public abstract void touch_end(LSL_Types.LSLInteger num_detected);

        public abstract void touch_start(LSL_Types.LSLInteger num_detected);

        public abstract void transaction_result(LSL_Types.key id, LSL_Types.LSLInteger success, LSL_Types.LSLString data);

        public abstract void no_sensor();


        public abstract void email(LSL_Types.LSLString time, LSL_Types.LSLString address, LSL_Types.LSLString subject,
            LSL_Types.LSLString message, LSL_Types.LSLInteger num_left);


        public abstract void http_request(LSL_Types.key request_id, LSL_Types.LSLString method, LSL_Types.LSLString body);

        public abstract void object_rez(LSL_Types.key id);

        public abstract void money(LSL_Types.key id, LSL_Types.LSLInteger amount);

        public abstract void moving_end();

        public abstract void moving_start();

        public abstract void state_exit();


        public abstract void llLinkSitTarget(LSL_Types.LSLInteger link, LSL_Types.Vector3 offset,
            LSL_Types.Quaternion rot);


        public abstract LSL_Types.LSLString llList2CSV(LSL_Types.list src);

        public abstract LSL_Types.LSLFloat llList2Float(LSL_Types.list src, LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLInteger llList2Integer(LSL_Types.list src, LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLString llList2Json(LSL_Types.LSLString type, LSL_Types.list values);

        public abstract LSL_Types.key llList2Key(LSL_Types.list src, LSL_Types.LSLInteger index);


        public abstract LSL_Types.list llList2List(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger endIndex);


        public abstract LSL_Types.list llList2ListStrided(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger endIndex, LSL_Types.LSLInteger stride);


        public abstract LSL_Types.Quaternion llList2Rot(LSL_Types.list src, LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLString llList2String(LSL_Types.list src, LSL_Types.LSLInteger index);

        public abstract LSL_Types.Vector3 llList2Vector(LSL_Types.list src, LSL_Types.LSLInteger index);


        public abstract LSL_Types.LSLInteger llListen(LSL_Types.LSLInteger channel, LSL_Types.LSLString name,
            LSL_Types.key id, LSL_Types.LSLString msg);


        public abstract void llListenControl(LSL_Types.LSLInteger handle, LSL_Types.LSLInteger active);

        public abstract void llListenRemove(LSL_Types.LSLInteger handle);

        public abstract LSL_Types.LSLInteger llListFindList(LSL_Types.list src, LSL_Types.list test);


        public abstract LSL_Types.list llListInsertList(LSL_Types.list dest, LSL_Types.list src,
            LSL_Types.LSLInteger start);


        public abstract LSL_Types.list llListRandomize(LSL_Types.list src, LSL_Types.LSLInteger stride);


        public abstract LSL_Types.list llListReplaceList(LSL_Types.list dest, LSL_Types.list src,
            LSL_Types.LSLInteger start, LSL_Types.LSLInteger endIndex);


        public abstract LSL_Types.list llListSort(LSL_Types.list src, LSL_Types.LSLInteger stride,
            LSL_Types.LSLInteger ascending);


        public abstract LSL_Types.LSLFloat llListStatistics(LSL_Types.LSLInteger operation, LSL_Types.list src);

        public abstract void llLoadURL(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.LSLString url);

        public abstract LSL_Types.LSLFloat llLog(LSL_Types.LSLFloat val);

        public abstract LSL_Types.LSLFloat llLog10(LSL_Types.LSLFloat val);

        public abstract void llLookAt(LSL_Types.Vector3 target, LSL_Types.LSLFloat strength, LSL_Types.LSLFloat damping);

        public abstract void llLoopSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);

        public abstract void llLoopSoundMaster(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);

        public abstract void llLoopSoundSlave(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);


        public abstract void llMakeExplosion(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture,
            LSL_Types.Vector3 offset);


        public abstract void llMakeFire(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale, LSL_Types.LSLFloat vel,
            LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture, LSL_Types.Vector3 offset);


        public abstract void llMakeFountain(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLInteger bounce,
            LSL_Types.LSLString texture, LSL_Types.Vector3 offset, LSL_Types.LSLFloat bounce_offset);


        public abstract void llMakeSmoke(LSL_Types.LSLInteger particles, LSL_Types.LSLFloat scale,
            LSL_Types.LSLFloat vel, LSL_Types.LSLFloat lifetime, LSL_Types.LSLFloat arc, LSL_Types.LSLString texture,
            LSL_Types.Vector3 offset);


        public abstract LSL_Types.LSLInteger llManageEstateAccess(LSL_Types.LSLInteger action, LSL_Types.key avatar);


        public abstract void llMapDestination(LSL_Types.LSLString simname, LSL_Types.Vector3 pos,
            LSL_Types.Vector3 look_at);


        public abstract LSL_Types.LSLString llMD5String(LSL_Types.LSLString src, LSL_Types.LSLInteger nonce);


        public abstract void llMessageLinked(LSL_Types.LSLInteger link, LSL_Types.LSLInteger num,
            LSL_Types.LSLString str, LSL_Types.key id);


        public abstract void llMinEventDelay(LSL_Types.LSLFloat delay);

        public abstract void llModifyLand(LSL_Types.LSLInteger action, LSL_Types.LSLInteger brush);


        public abstract LSL_Types.LSLInteger llModPow(LSL_Types.LSLInteger a, LSL_Types.LSLInteger b,
            LSL_Types.LSLInteger c);


        public abstract void llMoveToTarget(LSL_Types.Vector3 target, LSL_Types.LSLFloat tau);

        public abstract void llNavigateTo(LSL_Types.Vector3 pos, LSL_Types.list options);

        public abstract void llOffsetTexture(LSL_Types.LSLFloat u, LSL_Types.LSLFloat v, LSL_Types.LSLInteger face);

        public abstract void llOpenRemoteDataChannel();

        public abstract LSL_Types.LSLInteger llOverMyLand(LSL_Types.key id);

        public abstract void llOwnerSay(LSL_Types.LSLString msg);

        public abstract void llParcelMediaCommandList(LSL_Types.list commandList);

        public abstract LSL_Types.list llParcelMediaQuery(LSL_Types.list query);


        public abstract LSL_Types.list llParseString2List(LSL_Types.LSLString src, LSL_Types.list separators,
            LSL_Types.list spacers);


        public abstract LSL_Types.list llParseStringKeepNulls(LSL_Types.LSLString src, LSL_Types.list separators,
            LSL_Types.list spacers);


        public abstract void llParticleSystem(LSL_Types.list rules);

        public abstract void llLinkParticleSystem(LSL_Types.LSLInteger link, LSL_Types.list rules);

        public abstract void llPassCollisions(LSL_Types.LSLInteger pass);

        public abstract void llPassTouches(LSL_Types.LSLInteger pass);

        public abstract void llPatrolPoints(LSL_Types.list patrolPoints, LSL_Types.list options);

        public abstract void llPlaySound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);

        public abstract void llPlaySoundSlave(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);

        public abstract void llPointAt(LSL_Types.Vector3 pos);

        public abstract LSL_Types.LSLFloat llPow(LSL_Types.LSLFloat base_number, LSL_Types.LSLFloat exponent);

        public abstract void llPreloadSound(LSL_Types.LSLString sound);

        public abstract void llPursue(LSL_Types.key target, LSL_Types.list options);


        public abstract void llPushObject(LSL_Types.key target, LSL_Types.Vector3 impulse, LSL_Types.Vector3 ang_impulse,
            LSL_Types.LSLInteger local);


        public abstract void llRefreshPrimURL();

        public abstract void llRegionSay(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg);

        public abstract void llRegionSayTo(LSL_Types.key target, LSL_Types.LSLInteger channel, LSL_Types.LSLString msg);

        public abstract void llReleaseCamera(LSL_Types.key avatar);

        public abstract void llReleaseControls();

        public abstract void llReleaseURL(LSL_Types.LSLString url);


        public abstract void llRemoteDataReply(LSL_Types.key channel, LSL_Types.key message_id,
            LSL_Types.LSLString sdata, LSL_Types.LSLInteger idata);


        public abstract void llRemoteDataSetRegion();


        public abstract void llRemoteLoadScript(LSL_Types.key target, LSL_Types.LSLString name,
            LSL_Types.LSLInteger running, LSL_Types.LSLInteger start_param);


        public abstract void llRemoteLoadScriptPin(LSL_Types.key target, LSL_Types.LSLString name,
            LSL_Types.LSLInteger pin, LSL_Types.LSLInteger running, LSL_Types.LSLInteger start_param);


        public abstract void llRemoveFromLandBanList(LSL_Types.key avatar);

        public abstract void llRemoveFromLandPassList(LSL_Types.key avatar);

        public abstract void llRemoveInventory(LSL_Types.LSLString item);

        public abstract void llRemoveVehicleFlags(LSL_Types.LSLInteger flags);

        public abstract LSL_Types.key llRequestAgentData(LSL_Types.key id, LSL_Types.LSLInteger data);

        public abstract LSL_Types.key llRequestDisplayName(LSL_Types.key id);

        public abstract LSL_Types.key llRequestInventoryData(LSL_Types.LSLString name);

        public abstract void llRequestPermissions(LSL_Types.key agent, LSL_Types.LSLInteger permissions);

        public abstract LSL_Types.key llRequestSecureURL();

        public abstract LSL_Types.key llRequestSimulatorData(LSL_Types.LSLString region, LSL_Types.LSLInteger data);

        public abstract LSL_Types.key llRequestURL();

        public abstract LSL_Types.key llRequestUsername(LSL_Types.key id);

        public abstract void llResetAnimationOverride(LSL_Types.LSLString anim_state);

        public abstract void llResetLandBanList();

        public abstract void llResetLandPassList();

        public abstract void llResetOtherScript(LSL_Types.LSLString name);

        public abstract void llResetScript();

        public abstract void llResetTime();

        public abstract LSL_Types.LSLInteger llReturnObjectsByID(LSL_Types.list objects);

        public abstract LSL_Types.LSLInteger llReturnObjectsByOwner(LSL_Types.key owner, LSL_Types.LSLInteger scope);


        public abstract void llRezAtRoot(LSL_Types.LSLString inventory, LSL_Types.Vector3 position,
            LSL_Types.Vector3 velocity, LSL_Types.Quaternion rot, LSL_Types.LSLInteger param);


        public abstract void llRezObject(LSL_Types.LSLString inventory, LSL_Types.Vector3 pos, LSL_Types.Vector3 vel,
            LSL_Types.Quaternion rot, LSL_Types.LSLInteger param);


        public abstract LSL_Types.LSLFloat llRot2Angle(LSL_Types.Quaternion rot);

        public abstract LSL_Types.Vector3 llRot2Axis(LSL_Types.Quaternion rot);

        public abstract LSL_Types.Vector3 llRot2Euler(LSL_Types.Quaternion quat);

        public abstract LSL_Types.Vector3 llRot2Fwd(LSL_Types.Quaternion q);

        public abstract LSL_Types.Vector3 llRot2Left(LSL_Types.Quaternion q);

        public abstract LSL_Types.Vector3 llRot2Up(LSL_Types.Quaternion q);

        public abstract void llRotateTexture(LSL_Types.LSLFloat angle, LSL_Types.LSLInteger face);

        public abstract LSL_Types.Quaternion llRotBetween(LSL_Types.Vector3 start, LSL_Types.Vector3 endIndex);


        public abstract void llRotLookAt(LSL_Types.Quaternion target_direction, LSL_Types.LSLFloat strength,
            LSL_Types.LSLFloat damping);


        public abstract LSL_Types.LSLInteger llRotTarget(LSL_Types.Quaternion rot, LSL_Types.LSLFloat errorRadians);

        public abstract void llRotTargetRemove(LSL_Types.LSLInteger handle);

        public abstract LSL_Types.LSLInteger llRound(LSL_Types.LSLFloat val);

        public abstract LSL_Types.LSLInteger llSameGroup(LSL_Types.key uuid);

        public abstract void llSay(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg);

        public abstract LSL_Types.LSLInteger llScaleByFactor(LSL_Types.LSLFloat scaling_factor);

        public abstract void llScaleTexture(LSL_Types.LSLFloat u, LSL_Types.LSLFloat v, LSL_Types.LSLInteger face);

        public abstract LSL_Types.LSLInteger llScriptDanger(LSL_Types.Vector3 pos);

        public abstract void llScriptProfiler(LSL_Types.LSLInteger flags);


        public abstract LSL_Types.key llSendRemoteData(LSL_Types.key channel, LSL_Types.LSLString dest,
            LSL_Types.LSLInteger idata, LSL_Types.LSLString sdata);


        public abstract void llSensor(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger type,
            LSL_Types.LSLFloat range, LSL_Types.LSLFloat arc);


        public abstract void llSensorRemove();


        public abstract void llSensorRepeat(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger type,
            LSL_Types.LSLFloat range, LSL_Types.LSLFloat arc, LSL_Types.LSLFloat rate);


        public abstract void llSetAlpha(LSL_Types.LSLFloat alpha, LSL_Types.LSLInteger face);

        public abstract void llSetAngularVelocity(LSL_Types.Vector3 initial_omega, LSL_Types.LSLInteger local);

        public abstract void llSetAnimationOverride(LSL_Types.LSLString anim_state, LSL_Types.LSLString anim);

        public abstract void llSetBuoyancy(LSL_Types.LSLFloat buoyancy);

        public abstract void llSetCameraAtOffset(LSL_Types.Vector3 offset);

        public abstract void llSetCameraEyeOffset(LSL_Types.Vector3 offset);

        public abstract void llSetCameraParams(LSL_Types.list rules);

        public abstract void llSetClickAction(LSL_Types.LSLInteger action);

        public abstract void llSetColor(LSL_Types.Vector3 color, LSL_Types.LSLInteger face);

        public abstract void llSetContentType(LSL_Types.key request_id, LSL_Types.LSLInteger content_type);

        public abstract void llSetDamage(LSL_Types.LSLFloat damage);

        public abstract void llSetForce(LSL_Types.Vector3 force, LSL_Types.LSLInteger local);


        public abstract void llSetForceAndTorque(LSL_Types.Vector3 force, LSL_Types.Vector3 torque,
            LSL_Types.LSLInteger local);


        public abstract void llSetHoverHeight(LSL_Types.LSLFloat height, LSL_Types.LSLInteger water,
            LSL_Types.LSLFloat tau);


        public abstract void llSetInventoryPermMask(LSL_Types.LSLString item, LSL_Types.LSLInteger category,
            LSL_Types.LSLInteger value);


        public abstract void llSetKeyframedMotion(LSL_Types.list keyframes, LSL_Types.list options);


        public abstract void llSetLinkAlpha(LSL_Types.LSLInteger link, LSL_Types.LSLFloat alpha,
            LSL_Types.LSLInteger face);


        public abstract void llSetLinkCamera(LSL_Types.LSLInteger link, LSL_Types.Vector3 eye, LSL_Types.Vector3 at);


        public abstract void llSetLinkColor(LSL_Types.LSLInteger link, LSL_Types.Vector3 color,
            LSL_Types.LSLInteger face);


        public abstract LSL_Types.LSLInteger llSetLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face,
            LSL_Types.list parameters);


        public abstract void llSetLinkTexture(LSL_Types.LSLInteger link, LSL_Types.LSLString texture,
            LSL_Types.LSLInteger face);


        public abstract void llSetLinkTextureAnim(LSL_Types.LSLInteger link, LSL_Types.LSLInteger mode,
            LSL_Types.LSLInteger face, LSL_Types.LSLInteger sizex, LSL_Types.LSLInteger sizey, LSL_Types.LSLFloat start,
            LSL_Types.LSLFloat length, LSL_Types.LSLFloat rate);


        public abstract void llSetLocalRot(LSL_Types.Quaternion rot);

        public abstract LSL_Types.LSLInteger llSetMemoryLimit(LSL_Types.LSLInteger limit);

        public abstract void llSetObjectDesc(LSL_Types.LSLString description);

        public abstract void llSetObjectName(LSL_Types.LSLString name);

        public abstract void llSetObjectPermMask(LSL_Types.LSLInteger mask, LSL_Types.LSLInteger value);

        public abstract void llSetParcelMusicURL(LSL_Types.LSLString url);

        public abstract void llSetPayPrice(LSL_Types.LSLInteger price, LSL_Types.list quick_pay_buttons);


        public abstract void llSetPhysicsMaterial(LSL_Types.LSLInteger mask, LSL_Types.LSLFloat gravity_multiplier,
            LSL_Types.LSLFloat restitution, LSL_Types.LSLFloat friction, LSL_Types.LSLFloat density);


        public abstract void llSetPos(LSL_Types.Vector3 pos);

        public abstract void llSetPrimitiveParams(LSL_Types.list rules);

        public abstract void llSetLinkPrimitiveParams(LSL_Types.LSLInteger link, LSL_Types.list rules);

        public abstract void llSetLinkPrimitiveParamsFast(LSL_Types.LSLInteger link, LSL_Types.list rules);

        public abstract LSL_Types.LSLInteger llSetPrimMediaParams(LSL_Types.LSLInteger face, LSL_Types.list parameters);

        public abstract void llSetPrimURL(LSL_Types.LSLString url);

        public abstract LSL_Types.LSLInteger llSetRegionPos(LSL_Types.Vector3 position);

        public abstract void llSetRemoteScriptAccessPin(LSL_Types.LSLInteger pin);

        public abstract void llSetRot(LSL_Types.Quaternion rot);

        public abstract void llSetScale(LSL_Types.Vector3 size);

        public abstract void llSetScriptState(LSL_Types.LSLString name, LSL_Types.LSLInteger running);

        public abstract void llSetSitText(LSL_Types.LSLString text);

        public abstract void llSetSoundQueueing(LSL_Types.LSLInteger queue);

        public abstract void llSetSoundRadius(LSL_Types.LSLFloat radius);

        public abstract void llSetStatus(LSL_Types.LSLInteger status, LSL_Types.LSLInteger value);

        public abstract void llSetText(LSL_Types.LSLString text, LSL_Types.Vector3 color, LSL_Types.LSLFloat alpha);

        public abstract void llSetTexture(LSL_Types.LSLString texture, LSL_Types.LSLInteger face);


        public abstract void llSetTextureAnim(LSL_Types.LSLInteger mode, LSL_Types.LSLInteger face,
            LSL_Types.LSLInteger sizex, LSL_Types.LSLInteger sizey, LSL_Types.LSLFloat start, LSL_Types.LSLFloat length,
            LSL_Types.LSLFloat rate);


        public abstract void llSetTimerEvent(LSL_Types.LSLFloat sec);

        public abstract void llSetTorque(LSL_Types.Vector3 torque, LSL_Types.LSLInteger local);

        public abstract void llSetTouchText(LSL_Types.LSLString text);

        public abstract void llSetVehicleFlags(LSL_Types.LSLInteger flags);

        public abstract void llSetVehicleFloatParam(LSL_Types.LSLInteger param, LSL_Types.LSLFloat value);

        public abstract void llSetVehicleRotationParam(LSL_Types.LSLInteger param, LSL_Types.Quaternion rot);

        public abstract void llSetVehicleType(LSL_Types.LSLInteger type);

        public abstract void llSetVehicleVectorParam(LSL_Types.LSLInteger param, LSL_Types.Vector3 vec);

        public abstract void llSetVelocity(LSL_Types.Vector3 force, LSL_Types.LSLInteger local);

        public abstract LSL_Types.LSLString llSHA1String(LSL_Types.LSLString src);

        public abstract void llShout(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg);

        public abstract LSL_Types.LSLFloat llSin(LSL_Types.LSLFloat theta);

        public abstract void llSitTarget(LSL_Types.Vector3 offset, LSL_Types.Quaternion rot);

        public abstract void llSleep(LSL_Types.LSLFloat sec);


        public abstract void llSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume, LSL_Types.LSLInteger queue,
            LSL_Types.LSLInteger loopBool);


        public abstract void llSoundPreload(LSL_Types.LSLString sound);

        public abstract LSL_Types.LSLFloat llSqrt(LSL_Types.LSLFloat val);

        public abstract void llStartAnimation(LSL_Types.LSLString anim);

        public abstract void llStopAnimation(LSL_Types.LSLString anim);

        public abstract void llStopHover();

        public abstract void llStopLookAt();

        public abstract void llStopMoveToTarget();

        public abstract void llStopPointAt();

        public abstract void llStopSound();

        public abstract LSL_Types.LSLInteger llStringLength(LSL_Types.LSLString str);

        public abstract LSL_Types.LSLString llStringToBase64(LSL_Types.LSLString str);

        public abstract LSL_Types.LSLString llStringTrim(LSL_Types.LSLString src, LSL_Types.LSLInteger type);

        public abstract LSL_Types.LSLInteger llSubStringIndex(LSL_Types.LSLString source, LSL_Types.LSLString pattern);

        public abstract void llTakeCamera(LSL_Types.key avatar);


        public abstract void llTakeControls(LSL_Types.LSLInteger controls, LSL_Types.LSLInteger accept,
            LSL_Types.LSLInteger pass_on);


        public abstract LSL_Types.LSLFloat llTan(LSL_Types.LSLFloat theta);

        public abstract LSL_Types.LSLInteger llTarget(LSL_Types.Vector3 position, LSL_Types.LSLFloat range);

        public abstract void llTargetOmega(LSL_Types.Vector3 axis, LSL_Types.LSLFloat spinrate, LSL_Types.LSLFloat gain);

        public abstract void llTargetRemove(LSL_Types.LSLInteger handle);


        public abstract void llTeleportAgent(LSL_Types.key avatar, LSL_Types.LSLString landmark,
            LSL_Types.Vector3 position, LSL_Types.Vector3 look_at);


        public abstract void llTeleportAgentGlobalCoords(LSL_Types.key agent, LSL_Types.Vector3 global_coordinates,
            LSL_Types.Vector3 region_coordinates, LSL_Types.Vector3 look_at);


        public abstract void llTeleportAgentHome(LSL_Types.key avatar);

        public abstract void llTextBox(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.LSLInteger channel);

        public abstract LSL_Types.LSLString llToLower(LSL_Types.LSLString src);

        public abstract LSL_Types.LSLString llToUpper(LSL_Types.LSLString src);

        public abstract LSL_Types.key llTransferLindenDollars(LSL_Types.key destination, LSL_Types.LSLInteger amount);

        public abstract void llTriggerSound(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume);


        public abstract void llTriggerSoundLimited(LSL_Types.LSLString sound, LSL_Types.LSLFloat volume,
            LSL_Types.Vector3 top_north_east, LSL_Types.Vector3 bottom_south_west);


        public abstract LSL_Types.LSLString llUnescapeURL(LSL_Types.LSLString url);

        public abstract void llUnSit(LSL_Types.key id);

        public abstract void llUpdateCharacter(LSL_Types.list options);

        public abstract LSL_Types.LSLFloat llVecDist(LSL_Types.Vector3 vec_a, LSL_Types.Vector3 vec_b);

        public abstract LSL_Types.LSLFloat llVecMag(LSL_Types.Vector3 vec);

        public abstract LSL_Types.Vector3 llVecNorm(LSL_Types.Vector3 vec);

        public abstract void llVolumeDetect(LSL_Types.LSLInteger detect);

        public abstract void llWanderWithin(LSL_Types.Vector3 origin, LSL_Types.Vector3 dist, LSL_Types.list options);

        public abstract LSL_Types.LSLFloat llWater(LSL_Types.Vector3 offset);

        public abstract void llWhisper(LSL_Types.LSLInteger channel, LSL_Types.LSLString msg);

        public abstract LSL_Types.Vector3 llWind(LSL_Types.Vector3 offset);

        public abstract LSL_Types.LSLString llXorBase64(LSL_Types.LSLString str1, LSL_Types.LSLString str2);

        public abstract LSL_Types.LSLString llXorBase64Strings(LSL_Types.LSLString str1, LSL_Types.LSLString str2);

        public abstract LSL_Types.LSLString llXorBase64StringsCorrect(LSL_Types.LSLString str1, LSL_Types.LSLString str2);

        public abstract LSL_Types.LSLInteger llAbs(LSL_Types.LSLInteger val);

        public abstract LSL_Types.LSLFloat llAcos(LSL_Types.LSLFloat val);

        public abstract void llAddToLandBanList(LSL_Types.key avatar, LSL_Types.LSLFloat hours);

        public abstract void llAddToLandPassList(LSL_Types.key avatar, LSL_Types.LSLFloat hours);

        public abstract void llAdjustSoundVolume(LSL_Types.LSLFloat volume);

        public abstract void llAllowInventoryDrop(LSL_Types.LSLInteger add);

        public abstract LSL_Types.LSLFloat llAngleBetween(LSL_Types.Quaternion a, LSL_Types.Quaternion b);

        public abstract void llApplyImpulse(LSL_Types.Vector3 momentum, LSL_Types.LSLInteger local);

        public abstract void llApplyRotationalImpulse(LSL_Types.Vector3 force, LSL_Types.LSLInteger local);

        public abstract LSL_Types.LSLFloat llAsin(LSL_Types.LSLFloat val);

        public abstract LSL_Types.LSLFloat llAtan2(LSL_Types.LSLFloat y, LSL_Types.LSLFloat x);

        public abstract void llAttachToAvatar(LSL_Types.LSLInteger attach_point);

        public abstract void llAttachToAvatarTemp(LSL_Types.LSLInteger attach_point);

        public abstract LSL_Types.key llAvatarOnLinkSitTarget(LSL_Types.LSLInteger link);

        public abstract LSL_Types.key llAvatarOnSitTarget();


        public abstract LSL_Types.Quaternion llAxes2Rot(LSL_Types.Vector3 fwd, LSL_Types.Vector3 left,
            LSL_Types.Vector3 up);


        public abstract LSL_Types.Quaternion llAxisAngle2Rot(LSL_Types.Vector3 axis, LSL_Types.LSLFloat angle);

        public abstract LSL_Types.LSLInteger llBase64ToInteger(LSL_Types.LSLString str);

        public abstract LSL_Types.LSLString llBase64ToString(LSL_Types.LSLString str);

        public abstract void llBreakAllLinks();

        public abstract void llBreakLink(LSL_Types.LSLInteger link);

        public abstract LSL_Types.list llCSV2List(LSL_Types.LSLString src);

        public abstract LSL_Types.list llCastRay(LSL_Types.Vector3 start, LSL_Types.Vector3 endPoint, LSL_Types.list options);

        public abstract LSL_Types.LSLInteger llCeil(LSL_Types.LSLFloat val);

        public abstract void llClearCameraParams();

        public abstract LSL_Types.LSLInteger llClearLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face);

        public abstract LSL_Types.LSLInteger llClearPrimMedia(LSL_Types.LSLInteger face);

        public abstract void llCloseRemoteDataChannel(LSL_Types.key channel);

        public abstract LSL_Types.LSLFloat llCloud(LSL_Types.Vector3 offset);

        public abstract void llCollisionFilter(LSL_Types.LSLString name, LSL_Types.key id, LSL_Types.LSLInteger accept);

        public abstract void llCollisionSound(LSL_Types.LSLString impact_sound, LSL_Types.LSLFloat impact_volume);

        public abstract void llCollisionSprite(LSL_Types.LSLString impact_sprite);

        public abstract LSL_Types.LSLFloat llCos(LSL_Types.LSLFloat theta);

        public abstract void llCreateCharacter(LSL_Types.list options);

        public abstract void llCreateLink(LSL_Types.key target, LSL_Types.LSLInteger parent);

        public abstract void llDeleteCharacter();


        public abstract LSL_Types.list llDeleteSubList(LSL_Types.list src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger endIndex);


        public abstract LSL_Types.LSLString llDeleteSubString(LSL_Types.LSLString src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger endIndex);


        public abstract void llDetachFromAvatar();

        public abstract LSL_Types.Vector3 llDetectedGrab(LSL_Types.LSLInteger number);

        public abstract LSL_Types.LSLInteger llDetectedGroup(LSL_Types.LSLInteger number);

        public abstract LSL_Types.key llDetectedKey(LSL_Types.LSLInteger number);

        public abstract LSL_Types.LSLInteger llDetectedLinkNumber(LSL_Types.LSLInteger number);

        public abstract LSL_Types.LSLString llDetectedName(LSL_Types.LSLInteger item);

        public abstract LSL_Types.key llDetectedOwner(LSL_Types.LSLInteger number);

        public abstract LSL_Types.Vector3 llDetectedPos(LSL_Types.LSLInteger number);

        public abstract LSL_Types.Quaternion llDetectedRot(LSL_Types.LSLInteger number);

        public abstract LSL_Types.Vector3 llDetectedTouchBinormal(LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLInteger llDetectedTouchFace(LSL_Types.LSLInteger index);

        public abstract LSL_Types.Vector3 llDetectedTouchNormal(LSL_Types.LSLInteger index);

        public abstract LSL_Types.Vector3 llDetectedTouchPos(LSL_Types.LSLInteger index);

        public abstract LSL_Types.Vector3 llDetectedTouchST(LSL_Types.LSLInteger index);

        public abstract LSL_Types.Vector3 llDetectedTouchUV(LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLInteger llDetectedType(LSL_Types.LSLInteger number);

        public abstract LSL_Types.Vector3 llDetectedVel(LSL_Types.LSLInteger number);


        public abstract void llDialog(LSL_Types.key avatar, LSL_Types.LSLString message, LSL_Types.list buttons,
            LSL_Types.LSLInteger channel);


        public abstract void llDie();

        public abstract LSL_Types.LSLString llDumpList2String(LSL_Types.list src, LSL_Types.LSLString separator);

        public abstract LSL_Types.LSLInteger llEdgeOfWorld(LSL_Types.Vector3 pos, LSL_Types.Vector3 dir);

        public abstract void llEjectFromLand(LSL_Types.key avatar);


        public abstract void llEmail(LSL_Types.LSLString address, LSL_Types.LSLString subject,
            LSL_Types.LSLString message);


        public abstract LSL_Types.LSLString llEscapeURL(LSL_Types.LSLString url);

        public abstract LSL_Types.Quaternion llEuler2Rot(LSL_Types.Vector3 v);

        public abstract void llEvade(LSL_Types.key target, LSL_Types.list options);

        public abstract void llExecCharacterCmd(LSL_Types.LSLInteger command, LSL_Types.list options);

        public abstract LSL_Types.LSLFloat llFabs(LSL_Types.LSLFloat val);

        public abstract void llFleeFrom(LSL_Types.Vector3 position, LSL_Types.LSLFloat distance, LSL_Types.list options);

        public abstract LSL_Types.LSLInteger llFloor(LSL_Types.LSLFloat val);

        public abstract void llForceMouselook(LSL_Types.LSLInteger mouselook);

        public abstract LSL_Types.LSLFloat llFrand(LSL_Types.LSLFloat mag);

        public abstract LSL_Types.key llGenerateKey();

        public abstract LSL_Types.Vector3 llGetAccel();

        public abstract LSL_Types.LSLInteger llGetAgentInfo(LSL_Types.key id);

        public abstract LSL_Types.LSLString llGetAgentLanguage(LSL_Types.key avatar);

        public abstract LSL_Types.list llGetAgentList(LSL_Types.LSLInteger scope, LSL_Types.list options);

        public abstract LSL_Types.Vector3 llGetAgentSize(LSL_Types.key avatar);

        public abstract LSL_Types.LSLFloat llGetAlpha(LSL_Types.LSLInteger face);

        public abstract LSL_Types.LSLFloat llGetAndResetTime();

        public abstract LSL_Types.LSLString llGetAnimation(LSL_Types.key id);

        public abstract LSL_Types.list llGetAnimationList(LSL_Types.key avatar);

        public abstract LSL_Types.LSLString llGetAnimationOverride(LSL_Types.LSLString anim_state);

        public abstract LSL_Types.LSLInteger llGetAttached();

        public abstract LSL_Types.list llGetBoundingBox(LSL_Types.key object_key);

        public abstract LSL_Types.Vector3 llGetCameraPos();

        public abstract LSL_Types.Quaternion llGetCameraRot();

        public abstract LSL_Types.Vector3 llGetCenterOfMass();

        public abstract LSL_Types.list llGetClosestNavPoint(LSL_Types.Vector3 point, LSL_Types.list options);

        public abstract LSL_Types.Vector3 llGetColor(LSL_Types.LSLInteger face);

        public abstract LSL_Types.key llGetCreator();

        public abstract LSL_Types.LSLString llGetDate();

        public abstract LSL_Types.LSLString llGetDisplayName(LSL_Types.key id);

        public abstract LSL_Types.LSLFloat llGetEnergy();

        public abstract LSL_Types.LSLString llGetEnv(LSL_Types.LSLString name);

        public abstract LSL_Types.Vector3 llGetForce();

        public abstract LSL_Types.LSLInteger llGetFreeMemory();

        public abstract LSL_Types.LSLInteger llGetFreeURLs();

        public abstract LSL_Types.LSLFloat llGetGMTclock();

        public abstract LSL_Types.Vector3 llGetGeometricCenter();

        public abstract LSL_Types.LSLString llGetHTTPHeader(LSL_Types.key request_id, LSL_Types.LSLString header);

        public abstract LSL_Types.key llGetInventoryCreator(LSL_Types.LSLString item);

        public abstract LSL_Types.key llGetInventoryKey(LSL_Types.LSLString name);

        public abstract LSL_Types.LSLString llGetInventoryName(LSL_Types.LSLInteger type, LSL_Types.LSLInteger number);

        public abstract LSL_Types.LSLInteger llGetInventoryNumber(LSL_Types.LSLInteger type);


        public abstract LSL_Types.LSLInteger llGetInventoryPermMask(LSL_Types.LSLString item,
            LSL_Types.LSLInteger category);


        public abstract LSL_Types.LSLInteger llGetInventoryType(LSL_Types.LSLString name);

        public abstract LSL_Types.key llGetKey();

        public abstract LSL_Types.key llGetLandOwnerAt(LSL_Types.Vector3 pos);

        public abstract LSL_Types.key llGetLinkKey(LSL_Types.LSLInteger link);


        public abstract LSL_Types.list llGetLinkMedia(LSL_Types.LSLInteger link, LSL_Types.LSLInteger face,
            LSL_Types.list parameters);


        public abstract LSL_Types.LSLString llGetLinkName(LSL_Types.LSLInteger link);

        public abstract LSL_Types.LSLInteger llGetLinkNumber();

        public abstract LSL_Types.LSLInteger llGetLinkNumberOfSides(LSL_Types.LSLInteger link);

        public abstract LSL_Types.list llGetPrimitiveParams(LSL_Types.list parameters);

        public abstract LSL_Types.LSLInteger llGetListEntryType(LSL_Types.list src, LSL_Types.LSLInteger index);

        public abstract LSL_Types.LSLInteger llGetListLength(LSL_Types.list src);

        public abstract LSL_Types.Vector3 llGetLocalPos();

        public abstract LSL_Types.Quaternion llGetLocalRot();

        public abstract LSL_Types.LSLFloat llGetMass();

        public abstract LSL_Types.LSLFloat llGetMassMKS();

        public abstract LSL_Types.LSLFloat llGetMaxScaleFactor();

        public abstract LSL_Types.LSLInteger llGetMemoryLimit();

        public abstract LSL_Types.LSLFloat llGetMinScaleFactor();

        public abstract void llGetNextEmail(LSL_Types.LSLString address, LSL_Types.LSLString subject);

        public abstract LSL_Types.key llGetNotecardLine(LSL_Types.LSLString name, LSL_Types.LSLInteger line);

        public abstract LSL_Types.key llGetNumberOfNotecardLines(LSL_Types.LSLString name);

        public abstract LSL_Types.LSLInteger llGetNumberOfPrims();

        public abstract LSL_Types.LSLInteger llGetNumberOfSides();

        public abstract LSL_Types.LSLString llGetObjectDesc();

        public abstract LSL_Types.list llGetObjectDetails(LSL_Types.key id, LSL_Types.list parameters);

        public abstract LSL_Types.LSLFloat llGetObjectMass(LSL_Types.key id);

        public abstract LSL_Types.LSLString llGetObjectName();

        public abstract LSL_Types.LSLInteger llGetObjectPermMask(LSL_Types.LSLInteger category);

        public abstract LSL_Types.LSLInteger llGetObjectPrimCount(LSL_Types.key prim);

        public abstract LSL_Types.Vector3 llGetOmega();

        public abstract LSL_Types.key llGetOwner();

        public abstract LSL_Types.key llGetOwnerKey(LSL_Types.key id);

        public abstract LSL_Types.list llGetParcelDetails(LSL_Types.Vector3 pos, LSL_Types.list parameters);

        public abstract LSL_Types.LSLInteger llGetParcelFlags(LSL_Types.Vector3 pos);

        public abstract LSL_Types.LSLInteger llGetParcelMaxPrims(LSL_Types.Vector3 pos, LSL_Types.LSLInteger sim_wide);

        public abstract LSL_Types.LSLString llGetParcelMusicURL();


        public abstract LSL_Types.LSLInteger llGetParcelPrimCount(LSL_Types.Vector3 pos, LSL_Types.LSLInteger category,
            LSL_Types.LSLInteger sim_wide);


        public abstract LSL_Types.list llGetParcelPrimOwners(LSL_Types.Vector3 pos);

        public abstract LSL_Types.LSLInteger llGetPermissions();

        public abstract LSL_Types.key llGetPermissionsKey();

        public abstract LSL_Types.list llGetPhysicsMaterial();

        public abstract LSL_Types.Vector3 llGetPos();

        public abstract LSL_Types.list llGetPrimMediaParams(LSL_Types.LSLInteger face, LSL_Types.list parameters);

        public abstract LSL_Types.list llGetLinkPrimitiveParams(LSL_Types.LSLInteger link, LSL_Types.list parameters);

        public abstract LSL_Types.LSLInteger llGetRegionAgentCount();

        public abstract LSL_Types.Vector3 llGetRegionCorner();

        public abstract LSL_Types.LSLFloat llGetRegionFPS();

        public abstract LSL_Types.LSLInteger llGetRegionFlags();

        public abstract LSL_Types.LSLString llGetRegionName();

        public abstract LSL_Types.LSLFloat llGetRegionTimeDilation();

        public abstract LSL_Types.Vector3 llGetRootPosition();

        public abstract LSL_Types.Quaternion llGetRootRotation();

        public abstract LSL_Types.Quaternion llGetRot();

        public abstract LSL_Types.LSLInteger llGetSPMaxMemory();

        public abstract LSL_Types.Vector3 llGetScale();

        public abstract LSL_Types.LSLString llGetScriptName();

        public abstract LSL_Types.LSLInteger llGetScriptState(LSL_Types.LSLString script);

        public abstract LSL_Types.LSLFloat llGetSimStats(LSL_Types.LSLInteger stat_type);

        public abstract LSL_Types.LSLString llGetSimulatorHostname();

        public abstract LSL_Types.LSLInteger llGetStartParameter();


        public abstract LSL_Types.list llGetStaticPath(LSL_Types.Vector3 start, LSL_Types.Vector3 endPoint,
            LSL_Types.LSLFloat radius, LSL_Types.list parameters);


        public abstract LSL_Types.LSLInteger llGetStatus(LSL_Types.LSLInteger status);


        public abstract LSL_Types.LSLString llGetSubString(LSL_Types.LSLString src, LSL_Types.LSLInteger start,
            LSL_Types.LSLInteger endIndex);


        public abstract LSL_Types.Vector3 llGetSunDirection();

        public abstract LSL_Types.LSLString llGetTexture(LSL_Types.LSLInteger face);

        public abstract LSL_Types.Vector3 llGetTextureOffset(LSL_Types.LSLInteger face);

        public abstract LSL_Types.LSLFloat llGetTextureRot(LSL_Types.LSLInteger face);

        public abstract LSL_Types.Vector3 llGetTextureScale(LSL_Types.LSLInteger face);

        public abstract LSL_Types.LSLFloat llGetTime();

        public abstract LSL_Types.LSLFloat llGetTimeOfDay();

        public abstract LSL_Types.LSLString llGetTimestamp();

        public abstract LSL_Types.Vector3 llGetTorque();

        public abstract LSL_Types.LSLInteger llGetUnixTime();

        public abstract LSL_Types.LSLInteger llGetUsedMemory();

        public abstract LSL_Types.LSLString llGetUsername(LSL_Types.key id);

        public abstract LSL_Types.Vector3 llGetVel();

        public abstract LSL_Types.LSLFloat llGetWallclock();

        public abstract void llGiveInventory(LSL_Types.key destination, LSL_Types.LSLString inventory);


        public abstract void llGiveInventoryList(LSL_Types.key target, LSL_Types.LSLString folder,
            LSL_Types.list inventory);


        public abstract LSL_Types.LSLInteger llGiveMoney(LSL_Types.key destination, LSL_Types.LSLInteger amount);

        public abstract void llGodLikeRezObject(LSL_Types.key inventory, LSL_Types.Vector3 pos);

        public abstract LSL_Types.LSLFloat llGround(LSL_Types.Vector3 offset);

        public abstract LSL_Types.Vector3 llGroundContour(LSL_Types.Vector3 offset);

        public abstract LSL_Types.Vector3 llGroundNormal(LSL_Types.Vector3 offset);

        public abstract void llGroundRepel(LSL_Types.LSLFloat height, LSL_Types.LSLInteger water, LSL_Types.LSLFloat tau);

        public abstract LSL_Types.Vector3 llGroundSlope(LSL_Types.Vector3 offset);


        public abstract LSL_Types.key llHTTPRequest(LSL_Types.LSLString url, LSL_Types.list parameters,
            LSL_Types.LSLString body);


        public abstract void llHTTPResponse(LSL_Types.key request_id, LSL_Types.LSLInteger status,
            LSL_Types.LSLString body);


        public abstract LSL_Types.LSLString llInsertString(LSL_Types.LSLString dst, LSL_Types.LSLInteger pos,
            LSL_Types.LSLString src);


        public abstract void llInstantMessage(LSL_Types.key user, LSL_Types.LSLString message);

        public abstract LSL_Types.LSLString llIntegerToBase64(LSL_Types.LSLInteger number);

        public abstract LSL_Types.list llJson2List(LSL_Types.LSLString src);

        public abstract LSL_Types.LSLString llJsonGetValue(LSL_Types.LSLString json, LSL_Types.list specifiers);


        public abstract LSL_Types.LSLString llJsonSetValue(LSL_Types.LSLString json, LSL_Types.list specifiers,
            LSL_Types.LSLString value);


        public abstract LSL_Types.LSLString llJsonValueType(LSL_Types.LSLString json, LSL_Types.list specifiers);

        public abstract LSL_Types.LSLString llKey2Name(LSL_Types.key id);
    }
}