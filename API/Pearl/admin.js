import { Database, Pair } from './db.connect.js';

export class Administration{

    // ADMIN ONLY
    static Read = class{
        static async Admins(){
            const admins = await Database.Get.Table("admins", "id");
            let admin_info = [];
            for(let admin in admins){
                const info = (await Database.Get.Field("users", "username", new Pair("id", admins[admin].id)))[0];
                admin_info.push(info);
            }
            return admin_info;
        }
        static async Users(){
            const users = await Database.Get.Table("users", "id");
            let user_info = [];

            for(let user in users){
                user_info.push({id: users[user].id, name: users[user].username, is_admin: await Database.Get.Admin(users[user].id)});
            }

            return user_info;
        }
    }
    static async Create(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);
        if(!is_admin) return null;
            
        const data = {
            "username": body.username,
            "password": body.password,
        };       

        const user = await Database.Create.Entry("users", data);
        if(body.is_admin) {
            Database.Create.Entry("admins", {id: user.id}, null);
        }
    }

    static async Update(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);
        if(!is_admin) return null;

        if(body.id==null){
            body.id = (await Database.Get.Field("users", "id", new Pair("username", body.username)))[0].id;
        }
        const condition = new Pair("id", body.id);

        let data = {
            "username": body.username,
        };

        if(body.password!=null){
            data.password = body.password;
        }
        
        // Update admin status
        if(body.is_admin!=null){
            const is_already_admin = await Database.Get.Admin(id);
            if(body.is_admin != is_already_admin){
                if(is_already_admin){
                    await Database.Delete.Entry("admins", null, condition, is_admin);
                }
                else await Database.Create.Entry("admins", {id: body.id}, null);
            }
        }
        
        await Database.Update.Entry("users", null, condition, data, is_admin);
    }
    static async Delete(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);
        if(!is_admin) return null;

        if(body.id==null){
            body.id = (await Database.Get.Field("users", "id", new Pair("username", body.username)))[0].id;
        }

        const condition = new Pair("id", body.id);

        if(Database.Get.Admin(body.id)){
            await Database.Delete.Entry("admins", null, condition, is_admin);
        }
        await Database.Delete.Entry("users", null, condition, is_admin);
    }
}
