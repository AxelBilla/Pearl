public static class API {
    private static string address;
    private static int port;

    public static string user = "N/A";
    private static string password;


    public static class Create {
        public static bool Message(Message.Data message) {
            // Add message to DB via API call
            return true;
        }
    }

    public static class Read {
        public static Message.Data[] Messages() {
            //string messages = ""; // Get JSON from API call
            //return JSON.Get<Message.Data[]>(messages);
            return null;
        }
    }

    public static class Update {
        public static bool Message(Message.Data message) {
            // Update message from DB via API call
            return true;
        }
    }

    public static class Delete {
        public static bool Message(Message.Data message) {
            // Delete message from DB via API call
            return true;
        }
    }
}
