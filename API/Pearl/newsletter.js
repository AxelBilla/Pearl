import { Database, Pair } from './db.connect.js';

export class Newsletter{
    static async Read(){
        let articles = await Database.Get.Table("articles", "timestamp", -1);
        for(let index in articles){
            articles[index].timestamp = new Date(articles[index].timestamp).getTime();
            articles[index].last_update = new Date(articles[index].last_update).getTime();
            articles[index].category = (await Database.Get.Table("categories", "id", -1, new Pair("id", articles[index].category_id)))[0].name;

            const source_id = articles[index].source;
            const links = await Database.Get.Table("links", "article_id", -1, new Pair("article_id", articles[index].id));

            let sources = [];
            for(let link in links){
                let source = (await Database.Get.Table("sources", "id", -1, new Pair("id", links[link].source_id)))[0]; 
                delete source.id;

                sources.push(source);
            }

            articles[index].sources = sources;
        }
        return articles;
    }

    // ADMIN ONLY
    static async Create(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);
        if(!is_admin) return null;
        
        const data = {
            "content": body.content,
            "timestamp": Date.now(),
            "category_id": (await Database.Get.Field("categories", "id", new Pair("name", body.category)))[0].id,
            "admin_id": id
        };      

        const entry = await Database.Create.Entry("articles", data);
        if(body.sources!=null){
            for(let source in body.sources){
                    const data_source = {
                        "link": body.sources[source].link,
                        "article_id": entry.id,
                        "source_id": (await Database.Get.Field("sources", "id", new Pair("name", body.sources[source].name)))[0].id,
                    }
                    await Database.Create.Entry("links", data_source, null);
            }
        }
        return entry;
    }
    //static async Update(body, auth){
    //    let id = await Database.Get.ID(auth);
    //    const is_admin = await Database.Get.Admin(id);
    //    if(!is_admin) return null;
    //
    //    const condition = new Pair("id", body.id);
    //
    //    const data = {
    //        "content": body.content,
    //        "last_update": Date.now(),
    //        "category": await Database.Get.Table("categories", "id", 1, new Pair("name", body.category)),
    //    };
    //
    //    return Database.Update.Entry("articles", null, condition, data, is_admin);
    //}
    static async Delete(body, auth){
        const id = await Database.Get.ID(auth);
        const is_admin = await Database.Get.Admin(id);
        if(!is_admin) return null;

        const condition = new Pair("id", body.id);
        await Database.Delete.Entry("links", null, new Pair("article_id", body.id), is_admin);
        return Database.Delete.Entry("articles", null, condition, is_admin);
    }
}
