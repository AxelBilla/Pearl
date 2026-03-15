import { Database } from './db.connect.js';

export class Authentication{
    static ToJSON(headers){
        let auth = headers.authorization;
        auth = auth.split(" ")[1]; 
        return JSON.parse(atob(auth));
    }
    static async HasAccess(auth){
        return await Database.Get.Access(auth);
    }
    static async Information(auth){
        const user_id = await Database.Get.ID(auth);
        return {id: user_id, is_admin: await Database.Get.Admin(user_id)};
    }
}
