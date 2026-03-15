import { Database, Pair } from './db.connect.js';

export class Messages{
    static async Read(){
        let message_list = await Database.Get.Table("messages", "timestamp", -1);
        for(let index in message_list){
            message_list[index].username = await Database.Get.Name(message_list[index].user_id);
            message_list[index].timestamp = new Date(message_list[index].timestamp).getTime();
            message_list[index].last_update = new Date(message_list[index].last_update).getTime();

        }
        return message_list;
    }
    static async Create(body, auth){
        console.log(body);
        const id = await Database.Get.ID(auth);
    
        const data = {
            "content": body.content,
            "timestamp": Date.now(),
            "last_update": Date.now(),
            "user_id": id
        };       

        return Database.Create.Entry("messages", data);
    }
    static async Update(body, auth){
        let id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);

        const condition = new Pair("id", body.id);
        const check_id = new Pair("user_id", id);


        const data = {
            "content": body.content,
            "last_update": Date.now()
        };

        return Database.Update.Entry("messages", check_id, condition, data, is_admin);
    }
    static async Delete(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);

        const condition = new Pair("id", body.id);
        const check_id = new Pair("user_id", id);

        return Database.Delete.Entry("messages", check_id, condition, is_admin);
    }
}
