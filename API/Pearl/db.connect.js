import postgres from 'postgres'
import crypto from 'crypto';


import { createRequire } from "module";
const require = createRequire(import.meta.url);
const bcrypt = require('bcrypt');

const fs = require('node:fs');
const credentials = JSON.parse(fs.readFileSync('Pearl/.env/db.env.json', 'utf8'));

const sql = postgres({
    host                 : 'localhost',            // Postgres ip address[s] or domain name[s]
    port                 : 5432,          // Postgres server port[s]
    database             : credentials.name,            // Name of database to connect to
    username             : credentials.name,            // Username of database user
    password             : credentials.password,            // Password of database user
})
export class Database{
    static Get = class{
        static async Table(table, order_by, limit = 20, condition){
            if(condition!=null){
                if(limit>0){
                    return await sql`
                        SELECT *
                        FROM ${sql(table)}
                        WHERE ${sql(condition.property)} = ${condition.value}
                        ORDER BY ${sql(order_by)} DESC LIMIT ${limit}
                        `
                }
                else
                {
                    return await sql`
                        SELECT *
                        FROM ${sql(table)}
                        WHERE ${sql(condition.property)} = ${condition.value}
                        ORDER BY ${sql(order_by)} DESC
                        `
                }
            }
            else{
                if(limit>0) {
                    return await sql`
                        SELECT *
                        FROM ${sql(table)}
                        ORDER BY ${sql(order_by)} DESC LIMIT ${limit}
                    `
                } else {
                    return await sql`
                        SELECT * FROM ${sql(table)}
                        ORDER BY ${sql(order_by)} DESC
                    `
                }
            }
        }
        static async Field(table, field, condition){
            return await sql`
                SELECT ${sql(field)}
                FROM ${sql(table)}
                WHERE ${sql(condition.property)} = ${condition.value}
            `
        }

        static async Access(auth){
            const users = await Database.Get.Field("users", "password", new Pair("username", auth.username));
            for(let user in users){
                if(await bcrypt.compare(auth.password, users[user].password)) return true;
            }
            return false;
        }
        static async ID(auth){
            const users = await Database.Get.Table("users", "id", 1, new Pair("username", auth.username));
            return users[0].id;
        }
        static async Name(id){
            return (await Database.Get.Field("users", "username", new Pair("id", id)))[0].username;
        }
        static async Admin(id){
            const admins = await Database.Get.Field("admins", "id", new Pair("id", id));
            return (admins.length>0);
        }

    }
    static Delete = class{
        static async Entry(table, owner, condition, is_admin){
            if(is_admin){
                await sql`
                    DELETE  FROM ${sql(table)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                `
            }
            else{
                await sql`
                    DELETE  FROM ${sql(table)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                    AND ${sql(owner.property)} = ${owner.value}
                `
            }
        }
    }
    static Update = class{
        static async Entry(table, owner, condition, data, is_admin){
            if(is_admin){
                await sql`
                    UPDATE ${sql(table)}
                    SET ${sql(data)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                `
            }
            else{
                await sql`
                    UPDATE ${sql(table)}
                    SET ${sql(data)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                    AND ${sql(owner.property)} = ${owner.value}
                `
            }
            return data;
        }

    }
    static Create = class{
        static async Entry(table, data, custom_id = "id"){

            if(custom_id!=null && custom_id!=="") data[custom_id] = ID.Generate();

            await sql`
                    INSERT INTO ${sql(table)} ${sql(data)}
                `;
            return data;
        }

    }
}

export class Pair{
    constructor(property, value) {
        this.property = property;
        this.value = value;
    }
}

class ID{
    static Generate(){
        return crypto.randomUUID();
    }
}