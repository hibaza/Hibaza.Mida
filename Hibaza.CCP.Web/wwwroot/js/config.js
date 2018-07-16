// Language selector (front)
// LOCALES
var LANGUAGES_ACCEPTED = ['vi', 'en'];
var LOCALE_LANGUAGE = localStorage.getItem("lang");// window.navigator.userLanguage || window.navigator.language;
if (LANGUAGES_ACCEPTED.indexOf(LOCALE_LANGUAGE) < 0) {
    LOCALE_LANGUAGE = 'vi';
}
var DATE_FORMAT = (LOCALE_LANGUAGE == 'vi') ? "DD MMM YYYY" : "MMM Do, YYYY";
var LONG_DATE_FORMAT = (LOCALE_LANGUAGE == 'vi') ? "D MMMM YYYY, hh:mm" : " MMMM Do YYYY, hh:mm:ss";

var trAccountLocked = (LOCALE_LANGUAGE == 'en') ? 'Account locked' : 'Tài khoản bị khóa',
    trActivate = (LOCALE_LANGUAGE == 'en') ? 'Activate' : "Kích hoạt",
    trActivationPending = (LOCALE_LANGUAGE == 'en') ? 'Activation pending' : "Chờ kích hoạt",
    trActive = (LOCALE_LANGUAGE == 'en') ? 'Active' : "Hoạt động",
    trAddAnother = (LOCALE_LANGUAGE == 'en') ? 'Add another kind of question' : "Thêm một loại câu hỏi khác",
    trAddMore = (LOCALE_LANGUAGE == 'en') ? 'Add more' : "Thêm",
    trAlert = (LOCALE_LANGUAGE == 'en') ? 'Alert' : "Cảnh báo",
    trAll = (LOCALE_LANGUAGE == 'en') ? 'All' : "Tất cả",
    trAllAgents = (LOCALE_LANGUAGE == 'en') ? 'All agents' : "Tài khoản ủy quyền",
    trAnErrorOcurred = (LOCALE_LANGUAGE == 'en') ? 'An error ocurred while processing your request. Please, try again later or contact our support team.' : 'Đã xảy ra lỗi trong khi xử lý yêu cầu của bạn. Vui lòng thử lại sau hoặc liên hệ với nhóm hỗ trợ của chúng tôi. ',
    trAssign = (LOCALE_LANGUAGE == 'en') ? 'Assign' : "Phân tin",
    trAssigned = (LOCALE_LANGUAGE == 'en') ? 'Assigned' : "Đã phân tin",
    trAssignedMe = (LOCALE_LANGUAGE == 'en') ? 'Assigned to me' : "Đã phân tin cho tôi",
    trBusy = (LOCALE_LANGUAGE == 'en') ? 'Busy' : "Bận",
    trChatArchived = (LOCALE_LANGUAGE == 'en') ? 'Chat archived' : "Trò chuyện đã được lưu",
    trChatAssignedToOther = (LOCALE_LANGUAGE == 'en') ? 'This chat is assigned to other agent' : "Cuộc hội thoại này được gán cho người khác",
    trChatRemovedChannel = (LOCALE_LANGUAGE == 'en') ? 'The chat channel has been removed' : 'Cuộc hội thoại đã bị xóa',
    trChooseTheUser = (LOCALE_LANGUAGE == 'en') ? 'Choose the user field you want to store with this question' : 'Chọn trường người dùng bạn muốn lưu trữ với câu hỏi này',
    trCopied = (LOCALE_LANGUAGE == 'en') ? ' copied' : " đã copy",
    trClickOnTicket = (LOCALE_LANGUAGE == 'en') ? 'Click on the ticket name to view its details.' : 'Nhấp vào tên sự vụ để xem chi tiết của nó.',
    trCustomSelection = (LOCALE_LANGUAGE == 'en') ? 'Custom selection' : 'Lựa chọn',
    trDataSaved = (LOCALE_LANGUAGE == 'en') ? 'Data saved.' : 'Đã lưu dữ liệu',
    trDeletedAgent = (LOCALE_LANGUAGE == 'en') ? 'Deleted agent' : 'Đã xóa tài khoản ủy quyền',
    trDemands = (LOCALE_LANGUAGE == 'en') ? ' demands your product' : ' yêu cầu sản phẩm',
    trDoYouWant = (LOCALE_LANGUAGE == 'en') ? 'Do you want to store the answer in your reports?' : 'Bạn có muốn lưu câu trả lời trong báo cáo của mình không?',
    trEachTime = (LOCALE_LANGUAGE == 'en') ? 'Each time you start a conversation Hibaza create a ticket automatically to facilitate your work.' : 'Mỗi lần bạn bắt đầu một cuộc trò chuyện, Hibaza tự động tạo một vé để tạo điều kiện cho công việc của bạn.',
    trEdit = (LOCALE_LANGUAGE == 'en') ? 'Edit' : 'Chỉnh sửa',
    trEditQuestion = (LOCALE_LANGUAGE == 'en') ? 'Edit your question' : 'Chỉnh sửa câu hỏi của bạn',
    trEmail = 'Email',
    trError = (LOCALE_LANGUAGE == 'en') ?'ERROR':"LỖI",
    trErrorSendingNote = (LOCALE_LANGUAGE == 'en') ?'Error sending note':"Lỗi gửi ghi chú",
    trErrorSendingData = (LOCALE_LANGUAGE == 'en') ?'Error sending data':"Lỗi gửi dữ liệu",
    trErrorText = (LOCALE_LANGUAGE == 'en') ? 'Error text':"Văn bản lỗi",
    trExpiredAuth = (LOCALE_LANGUAGE == 'en') ?'Expired auth':"Xác thực đã hết bạn",
    trFastResponses = (LOCALE_LANGUAGE == 'en') ? 'Fast responses allow you to execute texts you frequently send to your customers.  ' : 'Phản hồi nhanh cho phép bạn thực thi các văn bản bạn thường xuyên gửi cho khách hàng của mình. ',
    trHasBeenCopied = (LOCALE_LANGUAGE == 'en') ? ' has been copied to your clipboard. Share it with your customers!' : ' đã được copy. Chia sẻ nó với khách hàng của bạn! ',
    trHasSet = (LOCALE_LANGUAGE == 'en') ? ' has set its ':'đã thiết lập',
    trHasStarted = (LOCALE_LANGUAGE == 'en') ? ' has started a conversation.' : ' đã bắt đầu một cuộc trò chuyện.',
    trHereIs = (LOCALE_LANGUAGE == 'en') ? 'Here is the list of channels and agents that will allow you to filter conversations.' : 'Dưới đây là danh sách các kênh và tài khoản ủy quyền sẽ cho phép bạn lọc các cuộc hội thoại.',
    trHey = (LOCALE_LANGUAGE == 'en') ?'Hey, ':"Xin chào",
    trHeyCustomer = (LOCALE_LANGUAGE == 'en') ? 'Hey! Your customer ' : 'Chào! Khách hàng của bạn ',
    trIfYouCheck = (LOCALE_LANGUAGE == 'en') ? 'If you check this box you can write a private note, only seen by you or your team.' : 'Nếu bạn chọn, là ghi chú riêng tư, chỉ bạn hoặc nhóm của bạn mới thấy.',
    trIfYouWant = (LOCALE_LANGUAGE == 'en') ? 'If you want to add more channels, click on the + icon.':'Nếu bạn muốn thêm nhiều kênh hơn, hãy nhấp vào biểu tượng dấu +.',
    trIsNotPossible = (LOCALE_LANGUAGE == 'en') ? 'Is not possible to add more yes/no questions on this depth level. Add it on higher levels or add another kind of questions.' : 'Không thể thêm nhiều câu hỏi có / không ở cấp độ này. Thêm nó ở cấp cao hơn hoặc thêm một loại câu hỏi khác. ',
    trIVeCreated = (LOCALE_LANGUAGE == 'en') ? 'I have created a ticket for you' + ' 😎' :"Tôi đã tạo một sự vụ cho bạn" + "😎",
    trLearn = (LOCALE_LANGUAGE == 'en') ? 'Learn to take advantage' : 'Học cách tận dụng',
    trName = (LOCALE_LANGUAGE == 'en') ?'Name':"Tên",
    trNoFileSelected = (LOCALE_LANGUAGE == 'en') ?'No file selected':"Chưa chọn file",
    trNoMatchesFound = (LOCALE_LANGUAGE == 'en') ?'No matches found':"Không tìm thấy đúng yêu cầu của bạn",
    trNoteDeleted = (LOCALE_LANGUAGE == 'en') ?'Note deleted':"Đã xóa ghi chú",
    trOk = 'OK',
    trOpen = (LOCALE_LANGUAGE == 'en') ?'Open':"Mở",
    trOpenAssign = (LOCALE_LANGUAGE == 'en') ? 'Open & Assign':"Mở và phân tin",
    trPaymentSent = (LOCALE_LANGUAGE == 'en') ? 'Payment sent':"Gửi thanh toán",
    trPleaseTryAgain = (LOCALE_LANGUAGE == 'en') ? 'Please, try it again':"Vui lòng thử lại lần sau",
    trPredefined = (LOCALE_LANGUAGE == 'en') ? 'Predefined' :"Được xác định trước",
    trProTip = (LOCALE_LANGUAGE == 'en') ? 'Pro tip' : 'Lời khuyên hay',
    trQuestion = (LOCALE_LANGUAGE == 'en') ? 'Question text' : 'Câu hỏi',
    trRefresh = (LOCALE_LANGUAGE == 'en') ? 'Please refresh the page to connect properly' : 'Vui lòng làm mới trang (F5) để kết nối lại',
    trRemoved = (LOCALE_LANGUAGE == 'en') ? 'CHANNEL REMOVED':"ĐÃ XÓA KÊNH",
    trRemoveItem = (LOCALE_LANGUAGE == 'en') ? 'Remove item' : "Loại bỏ mục",
    trRenewSubscription = (LOCALE_LANGUAGE == 'en') ? 'The admin of this account must proceed to renew the subscription to continue using Hibaza.' : 'Quản trị viên của tài khoản này phải tiếp tục gia hạn đăng ký để tiếp tục sử dụng Hibaza.',
    trSaveChanges = (LOCALE_LANGUAGE == 'en') ? 'Save changes':"Lưu kênh",
    trSaved = (LOCALE_LANGUAGE == 'en') ?'Saved':"Đã lưu",
    trSearch = (LOCALE_LANGUAGE == 'en') ? 'Search':"Tìm kiếm",
    trSelectChat = (LOCALE_LANGUAGE == 'en') ? 'Select a chat':"Chọn 1 cuộc hội thoại",
    trSelectChatPayment = (LOCALE_LANGUAGE == 'en') ? 'Select a chat to create a payment' : 'Chọn một hội thoại để tạo một khoản thanh toán',
    trSelectChatTicket = (LOCALE_LANGUAGE == 'en') ? 'Select a chat to create a ticket' : 'Chọn một cuộc hội thoại để tạo sự vụ',
    trSendPayment = (LOCALE_LANGUAGE == 'en') ? 'Send payment':"Gửi thanh toán",
    trSessionOpened = (LOCALE_LANGUAGE == 'en') ?'Session opened on another device':"Tài khoản của bạn đang được mở trên thiết bị khác",
    trSetShort = (LOCALE_LANGUAGE == 'en') ? 'Set a short answer' : 'Đặt câu trả lời ngắn',
    trSetYourFirst = (LOCALE_LANGUAGE == 'en') ? 'Set your first response now.' :"Đặt câu trả lời đầu tiên của bạn ngay bây giờ.",
    trShortcut = (LOCALE_LANGUAGE == 'en') ? 'Shortcut' : "Phím tắt",
    trShortcutRemoved = (LOCALE_LANGUAGE == 'en') ?'Shortcut removed' :"Đã xóa phím tắt",
    trShowMeHow = (LOCALE_LANGUAGE == 'en') ? 'Show me how' : "Hãy chỉ cho tôi phải làm gì",
    trSubscriptionExpired = (LOCALE_LANGUAGE == 'en') ? 'Subscription expired' : 'Đăng ký hết hạn',
    trText = (LOCALE_LANGUAGE == 'en') ?  'Text':"Văn bản",
    trThisIs = (LOCALE_LANGUAGE == 'en') ? 'This is Hibaza’s new menu: switch between Chats, Metrics, Chatbot creator or Account Settings.' : 'Đây là menu mới của Hibaza: chuyển đổi giữa Trò chuyện, Chỉ số, tạo Chatbot hoặc Cài đặt tài khoản.',
    trThisIsOur = (LOCALE_LANGUAGE == 'en') ? 'This is our brand new chatbox, where you type your messages to your customer (press enter to send a message).' : 'Đây là nơi bạn nhập tin nhắn cho khách hàng của mình (nhấn enter để gửi tin nhắn).',
    trTicketsSimple = (LOCALE_LANGUAGE == 'en') ? 'Tickets are a simple way to keep track of relevant information inside the conversation.' : 'Sự vụ là một cách đơn giản để theo dõi các thông tin liên quan bên trong cuộc hội thoại.',
    trTo = (LOCALE_LANGUAGE == 'en') ? ' to ':" tới ",
    trToday = (LOCALE_LANGUAGE == 'en') ? 'Today':"Hôm nay",
    trTopTenAgents = (LOCALE_LANGUAGE == 'en') ? 'Top ten agents':"10 tài khoản đúng đầu",
    trTopTenCustomers = (LOCALE_LANGUAGE == 'en') ? 'Top ten customers':"10 khách hàng đúng đầu",
    trTypeMessage = (LOCALE_LANGUAGE == 'en') ? 'Type a message...' : 'Gõ một tin nhắn...',
    trUnassign = (LOCALE_LANGUAGE == 'en') ? 'Unassign' : 'Bỏ gán',
    trUnassigned = (LOCALE_LANGUAGE == 'en') ?'Unassigned':"Chưa được gán",
    trViewTicket = (LOCALE_LANGUAGE == 'en') ? 'View ticket':"Xem sự vụ",
    trWeHaveAdded = (LOCALE_LANGUAGE == 'en') ? 'We have added some cool stuff to the interface that will help you being more productive!':'Chúng tôi đã thêm một số nội dung thú vị vào giao diện sẽ giúp bạn làm việc hiệu quả hơn!',
    trWelcome = (LOCALE_LANGUAGE == 'en') ?'Welcome to Hibaza!':"Chào mừng tới Hibaza",
    trWeLlActive = (LOCALE_LANGUAGE == 'en') ? 'We’ll active your channel as soon as possible. After that, we will launch your QR code for you to scan it using WhatsApp’s app from your device' : 'Chúng tôi sẽ kích hoạt kênh của bạn càng sớm càng tốt. Sau đó, chúng tôi sẽ khởi chạy mã QR của bạn để bạn quét mã bằng ứng dụng của WhatsApp từ thiết bị của bạn ',
    trWeVeSent = (LOCALE_LANGUAGE == 'en') ? 'We have sent you an email with the payment details and the invoice. If you do not receive it, please contact our team.' : 'Chúng tôi đã gửi cho bạn một email với các chi tiết thanh toán và hóa đơn. Nếu bạn không nhận được, vui lòng liên hệ với nhóm của chúng tôi. ',
    trWriteNote = (LOCALE_LANGUAGE == 'en') ? 'Write your note and press enter (not visible by your customer).' : 'Viết ghi chú của bạn và nhấn enter (không hiển thị tới khách hàng).',
    trYesterday = (LOCALE_LANGUAGE == 'en') ?'Yesterday':"Hôm qua",
    trYouCanAlsoView = (LOCALE_LANGUAGE == 'en') ? 'You can also view the notes, starred messages and files exchanged in the conversation associated with the ticket.' : 'Bạn cũng có thể xem ghi chú, tin nhắn được gắn dấu sao và được liên kết với sư vụ.',
    trYouCanEdit = (LOCALE_LANGUAGE == 'en') ? 'You can edit the title, add tags or a description.' : 'Bạn có thể chỉnh sửa tiêu đề, thêm thẻ hoặc mô tả.',
    trYouCanFilter = (LOCALE_LANGUAGE == 'en') ? 'You can filter the conversations depending on whether they are new, assigned to someone or archived.' : 'Bạn có thể lọc các cuộc hội thoại tùy thuộc vào việc họ có phải là người mới hay không, được chỉ định cho ai đó hoặc được lưu.',
    trYouCanView = (LOCALE_LANGUAGE == 'en') ? 'You can view all your customer tickets here.' : 'Bạn có thể xem tất cả các sự vụ ở đây.',
    trYouNeed = (LOCALE_LANGUAGE == 'en') ? 'You need to assignate the client to send him messages.' : 'Bạn cần phải chỉ định khách hàng gửi tin nhắn.',
    trYour = (LOCALE_LANGUAGE == 'en') ?'Your':"Bạn";


var staticCSSURL = "/css/site.css?v=1.1.3";
var staticZamiAvatar = "/partners/logo/hibaza.png";
var staticImgURL = "/images/";
var phonePrefixURL = "/js/phone.json";
var mediaURL = '/';

var defNotificationTimer = 2e3,
    longNotificationTimer = 4e3,
    fileUploadLimit = 6,
    TICKET_DESCRIPTION_LIMIT = 3e3,
    TICKET_TITLE_LIMIT = 100,
    TICKET_TAG_LIMIT = 25,
    limitMessages = 20,
    limitTimestampSameGroup = 60,
    limitContacts = 20,
    limitPages = 500,
    ACTIVATION_TIME = 3e4,
    preloaderLimit = 2e4,
    ADVANCED_FILTER_LIMIT = 12,
    ROWS_PER_PAGE = 20;

var AVATAR_COLOR = ["avatar-yellow", "avatar-red", "avatar-violet", "avatar-green", "avatar-light-violet"];

var content = null;

//var FIREBASE_APP = null, settingsUIRedirect = !1, tutorialActive = !1, lastChatView = "all", chatView = "all", chatViewAgent = !1, chatViewChannel = !1, currentChat = !1, currentChatRef = !1, currentTicket = !1, currentAgent = !1, viewProfile = "client", viewClientOption = "info", viewTicketSelected = !1, viewPaymentSelected = !1, viewAgentSelected = !1, viewAgentOption = "activity", closeEvent = !0, recordingAudio = !1, settingsView = "info", timeoutActivation = !1, reportManager = !1, dashboardManager = !1, lastMessageScroll = 0, howManyMessages = 0, lastMessageToScroll = !1, timeoutUiAutoLoadMessages = !1, timeoutUiAutoScrollMessages = !1, intervalUiAutoScrollMessages = !1, firstMessageToScroll = !1, shortcutsFocused = !1, typeareaFocused = !1, imgSendFlags = [], imgSendUrls = [], preloaderEnded = !1, currentLimitContacts = limitContacts, timeoutUiAutoLoadContacts = !1, contactStatusView = "all", lastContactScroll = 0;
var FIREBASE_APP = null, settingsUIRedirect = !1, tutorialActive = !1, lastChatView = "all", chatView = "all", chatViewAgent = !1, chatViewChannel = !1, currentChat = !1, currentChatRef = !1, currentTicket = !1, currentAgent = !1, viewProfile = "client", viewClientOption = "info", viewTicketSelected = !1, viewPaymentSelected = !1, viewAgentSelected = !1, viewAgentOption = "activity", closeEvent = !0, recordingAudio = !1, settingsView = "info", timeoutActivation = !1, reportManager = !1, dashboardManager = !1, lastMessageScroll = 0, howManyMessages = 0, lastMessageToScroll = !1, timeoutUiAutoLoadMessages = !1, timeoutUiAutoScrollMessages = !1, intervalUiAutoScrollMessages = !1, firstMessageToScroll = !1, shortcutsFocused = !1, typeareaFocused = !1, imgSendFlags = [], imgSendUrls = [], preloaderEnded = !1, currentLimitContacts = limitContacts, timeoutUiAutoLoadContacts = !1, contactStatusView = "all", lastContactScroll = 0;


var messageZamiTimestamp = !1,
    messageZamiLastChat = !1;


// Prices
var roninQuote = 0;
var telegramQuote = 0;
var whatsappQuote = 0;
var facebookQuote = 0;
var smoochQuote = 0;
var yearDiscount = 17;

//TODO
var RESPONSABILITY = ["Agent", "Client", "Supplier"];
var STATUS = ["Pending", "Attention", "Completed", "Rejected"];

var CONTACTS = {}, THREADS = {}, MESSAGES = {}, COUNTERS = {
    channels: {},
    agents: {},
    pending_unread: {},
    channels_unread: {},
    attention_unread: {}
};

// Trial


var pricingVolume = false;
var suscriptionTrial = true;
// Expired

var suscriptionExpired = false;
var wantsWhatsapp = false;

// Expired Today

var suscriptionExpiredToday = false;

var NP_PAYMENT = {
    title: trPaymentSent,
    message: trWeVeSent,
    icon: "icon-mail",
    countdown: null
},
    NP_DATA_SAVE = {
        title: trSaved,
        message: trDataSaved,
        icon: "icon-check",
        countdown: defNotificationTimer
    };